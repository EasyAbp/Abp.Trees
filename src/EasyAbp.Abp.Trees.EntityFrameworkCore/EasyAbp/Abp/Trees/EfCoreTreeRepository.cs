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
        protected IGuidGenerator GuidGenerator { get; }

        protected ITreeCodeGenerator TreeCodeGenerator { get; }

        protected virtual Action<TEntity> TraverseTreeAction
        {
            get { return (x) => { }; }
        }

        public EfCoreTreeRepository(
            IDbContextProvider<TDbContext> dbContextProvider,
            IGuidGenerator guidGenerator,
            ITreeCodeGenerator treeCodeGenerator)
            : base(dbContextProvider)
        {
            GuidGenerator = guidGenerator;
            TreeCodeGenerator = treeCodeGenerator;
        }

        public override IQueryable<TEntity> WithDetails()
        {
            if (AbpEntityOptions.DefaultWithDetailsFunc == null)
            {
                return GetQueryable().Include(x => x.Children);
            }

            return AbpEntityOptions.DefaultWithDetailsFunc(GetQueryable().Include(x => x.Children));
        }

        public async Task<List<TEntity>> GetChildrenAsync(Guid parentId, bool includeDetails = true, bool recursive = false, CancellationToken cancellationToken = default)
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

            var code = await GetCodeAsync(parentId, GetCancellationToken(cancellationToken));

            return includeDetails
                ? await WithDetails()
                    .Where(x => x.Code.StartsWith(code) && x.Id != parentId)
                    .OrderBy(x => x.Code)
                    .ToListAsync(GetCancellationToken(cancellationToken))
                : await GetQueryable()
                    .Where(x => x.Code.StartsWith(code) && x.Id != parentId)
                    .OrderBy(x => x.Code)
                    .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// The code of node inserted will update by database. and it's children will be inserted by parent (when autoSave is false, the modification of code of children will cause error after insert)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity.Id == Guid.Empty)
            {
                EntityHelper.TrySetId(entity, () => GuidGenerator.Create());
            }

            var code = await GetNextChildCodeAsync(entity.ParentId, GetCancellationToken(cancellationToken));
            entity.SetCode(code);

            await TraverseTreeAsync(entity, entity.Children, autoSave, GetCancellationToken(cancellationToken));
            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }

            entity = await base.InsertAsync(entity, autoSave, GetCancellationToken(cancellationToken));
            return entity;//await base.InsertAsync(entity, autoSave, cancellationToken);
        }

        //todo: not allow modify children
        public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var oldEntity = await FindAsync(entity.Id, cancellationToken: cancellationToken);
            if (oldEntity.ParentId == entity.Id)
            {
                await base.UpdateAsync(entity, autoSave, cancellationToken);
                return entity;
            }

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

        public override async Task DeleteAsync(Guid id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var children = await GetChildrenAsync(id, true, cancellationToken: GetCancellationToken(cancellationToken));

            foreach (var child in children)
            {
                await base.DeleteAsync(child, autoSave, cancellationToken);
            }

            await base.DeleteAsync(id, autoSave, cancellationToken);
        }

        protected virtual async Task<string> GetNextChildCodeAsync(Guid? parentId, CancellationToken cancellationToken = default)
        {
            if (!parentId.HasValue)
            {
                return TreeCodeGenerator.Create(1);
            }

            var children = await GetChildrenAsync(
                parentId.Value,
                false,
                false,
                GetCancellationToken(cancellationToken));
            var lastChild = children.LastOrDefault();

            if (lastChild != null)
            {
                return TreeCodeGenerator.Next(lastChild.Code);
            }
            else
            {
                var parentCode = await GetCodeAsync(parentId.Value, GetCancellationToken(cancellationToken));
                return TreeCodeGenerator.Append(parentCode, TreeCodeGenerator.Create(1));
            }
        }

        protected virtual async Task<string> GetCodeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetAsync(id, false, GetCancellationToken(cancellationToken));
            return entity.Code;
        }

        //todo: move to Management or SubClass
        protected virtual async Task ValidateEntityAsync(TEntity entity)
        {
            if (entity.ParentId.HasValue)
            {
                var siblings = (await GetChildrenAsync(entity.ParentId.Value))
                    .Where(ou => ou.Id != entity.Id)
                    .ToList();

                if (siblings.Any(x => x.DisplayName == entity.DisplayName))
                {
                    throw new DuplicateDisplayNameException();
                }
            }
        }

        protected virtual async Task TraverseTreeAsync(TEntity parent, ICollection<TEntity> children, bool autoSave = false, CancellationToken cancellationToken = default)
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
                await TraverseTreeAsync(c, c.Children, autoSave, cancellationToken);
            }
        }
    }
}
