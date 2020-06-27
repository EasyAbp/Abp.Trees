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
        protected TreeCodeGenerator TreeCodeGenerator => LazyGetRequiredService(ref _treeCodeGenerator);
        private TreeCodeGenerator _treeCodeGenerator;
        private readonly IGuidGenerator _guidGenerator;

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

        public EfCoreTreeRepository(
            IDbContextProvider<TDbContext> dbContextProvider,
            IGuidGenerator guidGenerator)
            : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }
        private async Task traverseTreeAsync(TEntity parent, ICollection<TEntity> children, bool autoSave = false, CancellationToken cancellationToken = default)
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
                    Volo.Abp.Domain.Entities.EntityHelper.TrySetId(c, () => _guidGenerator.Create());
                }
                var code = TreeCodeGenerator.AppendCode(parent.Code, TreeCodeGenerator.CreateCode(++index));
                c.SetCode(code);
                TraverseTreeAction?.Invoke(c);
                await traverseTreeAsync(c, c.Children, autoSave, cancellationToken);
            }
            
        }
        protected virtual Action<TEntity> TraverseTreeAction
        {
            get { return (x) => { }; }
        }

        public override IQueryable<TEntity> WithDetails()
        {
            if (AbpEntityOptions.DefaultWithDetailsFunc == null)
            {
                return GetQueryable().Include(x => x.Children);
            }

            return AbpEntityOptions.DefaultWithDetailsFunc(GetQueryable().Include(x => x.Children));
        }
        //to be inserted node will get code by db. and children nodes will be asseted by parent(if autoSave==false,modify code of children will error after insert)
        public async override Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity.Id == Guid.Empty)
            {
                Volo.Abp.Domain.Entities.EntityHelper.TrySetId(entity, () => _guidGenerator.Create());
            }
            var code = await GetNextChildCodeAsync(entity.ParentId);
            entity.SetCode(code);
            await traverseTreeAsync(entity, entity.Children, autoSave, cancellationToken);
            
            await DbContext.SaveChangesAsync();
            await base.InsertAsync(entity, autoSave, cancellationToken);
            return entity;//await base.InsertAsync(entity, autoSave, cancellationToken);
        }
        //todo: not allow modify children
        public async override Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var oldEntity = await this.FindAsync(entity.Id);
            if (oldEntity.ParentId == entity.Id)
            {
                await base.UpdateAsync(entity, autoSave, cancellationToken);
                return entity;
            }
            var parentId = entity.ParentId;
            //Should find children before Code change
            var children = await FindChildrenAsync(entity.Id, true);

            //Store old code of Tree
            var oldCode = oldEntity.Code;

            //Move Tree
            var code = await GetNextChildCodeAsync(parentId);
            entity.SetCode(code);

            //Update Children Codes
            foreach (var child in children)
            {
                var childCode = TreeCodeGenerator.AppendCode(entity.Code, TreeCodeGenerator.GetRelativeCode(child.Code, oldCode));
                child.SetCode(childCode);
            }
            await base.UpdateAsync(entity, autoSave, cancellationToken);
            return entity;
        }
        public async override Task DeleteAsync(Guid id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var children = await FindChildrenAsync(id, true);

            foreach (var child in children)
            {
                await base.DeleteAsync(child, autoSave, cancellationToken);
            }

            await base.DeleteAsync(id, autoSave, cancellationToken);
        }
        protected virtual async Task<TEntity> GetLastChildOrNullAsync(Guid? parentId)
        {
            var children = await this.Where(tree => tree.ParentId == parentId).ToListAsync();
            return children.OrderBy(c => c.Code).LastOrDefault();
        }
        protected virtual async Task<string> GetNextChildCodeAsync(Guid? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return TreeCodeGenerator.AppendCode(parentCode, TreeCodeGenerator.CreateCode(1));
            }

            return TreeCodeGenerator.CalculateNextCode(lastChild.Code);
        }
        protected virtual async Task<string> GetCodeAsync(Guid id)
        {
            return (await base.GetAsync(id)).Code;
        }
        //todo: move to Management or SubClass
        protected virtual async Task ValidateEntityAsync(TEntity entity)
        {

            var siblings = (await FindChildrenAsync(entity.ParentId))
                .Where(ou => ou.Id != entity.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == entity.DisplayName))
            {
                throw new DuplicateDisplayNameException();
            }
        }
        public async Task<List<TEntity>> FindChildrenAsync(Guid? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return await this.Where(x => x.ParentId == parentId)
                    .OrderBy(x=>x.Code)
                    .ToListAsync();
            }

            if (!parentId.HasValue)
            {
                return await this.ToListAsync();
            }

            var code = await GetCodeAsync(parentId.Value);

            return await this.Where(ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value)
                .OrderBy(x=>x.Code)
                .ToListAsync();
        }
    }
}
