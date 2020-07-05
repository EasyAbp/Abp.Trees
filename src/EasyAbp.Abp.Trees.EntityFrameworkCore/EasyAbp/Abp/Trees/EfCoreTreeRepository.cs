using EasyAbp.Abp.Trees.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace EasyAbp.Abp.Trees
{
    public class EfCoreTreeRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity, Guid>,
        ITreeRepository<TEntity>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<Guid>, ITree<TEntity>
    {
        private ITreeCodeGenerator _treeCodeGenerator;
        private IGuidGenerator _guidGenerator;

        protected ITreeCodeGenerator TreeCodeGenerator => LazyGetRequiredService(ref _treeCodeGenerator);
        protected IGuidGenerator GuidGenerator => LazyGetRequiredService(ref _guidGenerator);

        #region ioc Lazy loading

        protected readonly object ServiceProviderLock = new object();

        protected TService LazyGetRequiredService<TService>(ref TService reference)
            => LazyGetRequiredService(typeof(TService), ref reference);

        protected TRef LazyGetRequiredService<TRef>(Type serviceType, ref TRef reference)
        {
            if (reference == null)
            {
                lock (ServiceProviderLock)
                {
                    if (reference == null)
                    {
                        reference = (TRef)ServiceProvider.GetRequiredService(serviceType);
                    }
                }
            }

            return reference;
        }

        #endregion ioc Lazy loading

        //keep one param Constructor,for simplify Custom Repository
        public EfCoreTreeRepository(
            IDbContextProvider<TDbContext> dbContextProvider
            ) : base(dbContextProvider) { }

        public override IQueryable<TEntity> WithDetails()
        {
            if (AbpEntityOptions.DefaultWithDetailsFunc == null)
            {
                return GetQueryable().Include(x => x.Children);
            }

            return AbpEntityOptions.DefaultWithDetailsFunc(GetQueryable().Include(x => x.Children));
        }

        public async Task<List<TEntity>> GetChildrenAsync(Guid? parentId, bool includeDetails = true, bool recursive = false, CancellationToken cancellationToken = default)
        {
            if (!recursive)
            {
                return includeDetails
                    ? await WithDetails()
                        .Where(x => x.ParentId == parentId)
                        .OrderBy(x => x.Code)
                        .ToListAsync(GetCancellationToken(cancellationToken))
                    : await GetQueryable()
                        .Where(x => x.ParentId == parentId)
                        .OrderBy(x => x.Code)
                        .ToListAsync(GetCancellationToken(cancellationToken));
            }

            if (!parentId.HasValue)
            {
                return await this.ToListAsync(GetCancellationToken(cancellationToken));
            }

            var code = await GetCodeAsync(parentId.Value, GetCancellationToken(cancellationToken));

            return includeDetails
                ? await WithDetails()
                    .Where(x => x.Code.StartsWith(code) && x.Id != parentId.Value)
                    .OrderBy(x => x.Code)
                    .ToListAsync(GetCancellationToken(cancellationToken))
                : await GetQueryable()
                    .Where(x => x.Code.StartsWith(code) && x.Id != parentId.Value)
                    .OrderBy(x => x.Code)
                    .ToListAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual async Task<string> GetCodeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return (await base.GetAsync(id)).Code;
        }

        //to be inserted node will get code by db. and children nodes will be asseted by parent(if autoSave==false,modify code of children will error after insert)
        public async override Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity.Id == Guid.Empty)
            {
                EntityHelper.TrySetId(entity, () => GuidGenerator.Create());
            }
            var code = await GetNextChildCodeAsync(entity.ParentId, GetCancellationToken(cancellationToken));
            entity.SetCode(code);
            await TraverseTreeAsync(entity, entity.Children);

            entity = await base.InsertAsync(entity, autoSave, cancellationToken);
            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }

            return entity;
        }

        //todo: not allow modify children
        public async override Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var oldEntity = await this.FindAsync(entity.Id, cancellationToken: cancellationToken);
            if (oldEntity.ParentId == entity.ParentId)
            {
                await base.UpdateAsync(entity, autoSave, cancellationToken);
                return entity;
            }
            //do move
            var parentId = entity.ParentId;
            //Should find children before Code change
            var children = await GetChildrenAsync(entity.Id, true, cancellationToken: GetCancellationToken(cancellationToken));

            //Store old code of Tree
            var oldCode = oldEntity.Code;

            //Move Tree
            var code = await GetNextChildCodeAsync(parentId, GetCancellationToken(cancellationToken));
            entity.SetCode(code);

            //Update Children Codes
            foreach (var child in children)
            {
                var childCode = TreeCodeGenerator.Append(entity.Code, TreeCodeGenerator.GetRelative(child.Code, oldCode));
                child.SetCode(childCode);
            }
            await base.UpdateAsync(entity, autoSave, cancellationToken);
            return entity;
        }

        public async override Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            entity.ClearChildren();

            var children = await GetChildrenAsync(entity.Id, true, cancellationToken: GetCancellationToken(cancellationToken));

            foreach (var child in children)
            {
                await DeleteAsync(child, autoSave, cancellationToken);
            }

            await base.DeleteAsync(entity, autoSave, cancellationToken);
        }

        protected virtual async Task<string> GetNextChildCodeAsync(Guid? parentId, CancellationToken cancellationToken = default)
        {
            var children = await GetChildrenAsync(
                parentId,
                false,
                false,
                GetCancellationToken(cancellationToken));
            var lastChild = children.LastOrDefault();
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return TreeCodeGenerator.Append(parentCode, TreeCodeGenerator.Create(1));
            }

            return TreeCodeGenerator.Next(lastChild.Code);
        }

        protected virtual async Task TraverseTreeAsync(TEntity parent, ICollection<TEntity> children)
        {
            if (children == null || !children.Any())
            {
                return;
            }
            var index = 0;
            foreach (var c in children)
            {
                if (c.Id == Guid.Empty)
                {
                    EntityHelper.TrySetId(c, () => GuidGenerator.Create());
                }
                var code = TreeCodeGenerator.Append(parent.Code, TreeCodeGenerator.Create(++index));
                c.SetCode(code);
                TraverseTreeAction?.Invoke(c);
                await TraverseTreeAsync(c, c.Children);
            }
        }

        protected virtual Action<TEntity> TraverseTreeAction
        {
            get { return (x) => { }; }
        }

        //todo: move to Management or SubClass
        protected virtual async Task ValidateEntityAsync(TEntity entity)
        {
            var siblings = (await GetChildrenAsync(entity.ParentId))
                .Where(ou => ou.Id != entity.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == entity.DisplayName))
            {
                throw new DuplicateDisplayNameException();
            }
        }
    }
}