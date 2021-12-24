using EasyAbp.Abp.Trees.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Abp.Trees
{
    public static class EfCoreTreeQueryableExtensions
    {
        public static IQueryable<TEntity> IncludeDetails<TEntity>(this IQueryable<TEntity> queryable, bool include = true)
            where TEntity : class, ITree<TEntity>
        {
            if (!include)
            {
                return queryable;
            }
            return queryable
                .Include(x => x.Children);
        }
    }
    public class EfCoreTreeRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity, Guid>,
        ITreeRepository<TEntity>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<Guid>, ITree<TEntity>
    {
        protected ITreeCodeGenerator<TEntity> TreeCodeGenerator => this.LazyServiceProvider.LazyGetRequiredService<ITreeCodeGenerator<TEntity>>();

        public EfCoreTreeRepository(
            IDbContextProvider<TDbContext> dbContextProvider
            ) : base(dbContextProvider) { }

        public override async Task<IQueryable<TEntity>> WithDetailsAsync()
        {
            if (AbpEntityOptions.DefaultWithDetailsFunc == null)
            {
                return (await GetQueryableAsync()).IncludeDetails(true);
            }

            return AbpEntityOptions.DefaultWithDetailsFunc((await GetQueryableAsync()).IncludeDetails(true));
        }

        public async Task<List<TEntity>> GetChildrenAsync(Guid? parentId, bool includeDetails = true, bool recursive = false, CancellationToken cancellationToken = default)
        {
            if (!recursive)
            {

                return await (await WithDetailsAsync())
                    .Where(x => x.ParentId == parentId)
                    .OrderBy(x => x.Code)
                    .ToListAsync(GetCancellationToken(cancellationToken));
            }

            if (!parentId.HasValue)
            {
                return await (await this.GetQueryableAsync()).ToListAsync(GetCancellationToken(cancellationToken));
            }

            var code = await GetCodeAsync(parentId.Value, GetCancellationToken(cancellationToken));

            return await (await GetQueryableAsync()).IncludeDetails(includeDetails)
                    .Where(x => x.Code.StartsWith(code) && x.Id != parentId.Value)
                    .OrderBy(x => x.Code)
                    .ToListAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual async Task<string> GetCodeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return (await base.GetAsync(id)).Code;
        }

        public async override Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            CheckAndSetId(entity);

            var code = await GetNextChildCodeAsync(entity.ParentId, GetCancellationToken(cancellationToken));

            entity.SetCode(code);

            await TraverseTreeAsync(entity, entity.Children);

            return await base.InsertAsync(entity, autoSave, cancellationToken);
        }
        public override async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entityArray = entities.ToArray();

            foreach (var entity in entityArray)
            {
                await InsertAsync(entity, autoSave, cancellationToken);
            }
        }

        //todo: not allow modify children
        public async override Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var oldEntity = await (await this.GetQueryableAsync()).AsNoTracking().Where(x => x.Id == entity.Id).SingleOrDefaultAsync(cancellationToken);

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
            var dbContext = await GetDbContextAsync();

            var localChildren = dbContext.Set<TEntity>().Local
                .Where(x => x.ParentId == parentId)
                .Where(x => dbContext.Entry(x).State == EntityState.Added);

            var children = await GetChildrenAsync(
                parentId,
                false,
                false,
                GetCancellationToken(cancellationToken));

            children.AddRange(localChildren);

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
    }
}