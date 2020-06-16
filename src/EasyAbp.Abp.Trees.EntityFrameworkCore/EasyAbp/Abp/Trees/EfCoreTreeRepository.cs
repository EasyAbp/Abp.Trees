using EasyAbp.Abp.Trees.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Abp.Trees
{
    public class EfCoreTreeRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity, Guid>,
        ITreeRepository<TEntity> 
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<Guid>, ITree<TEntity>
    {
        protected TreeCodeDomainService TreeCodeDomainService => LazyGetRequiredService(ref _treeCodeDomainService);
        private TreeCodeDomainService _treeCodeDomainService;

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
            IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public override IQueryable<TEntity> WithDetails()
        {
            if (AbpEntityOptions.DefaultWithDetailsFunc == null)
            {
                return GetQueryable().Include(x => x.Children);
            }

            return AbpEntityOptions.DefaultWithDetailsFunc(GetQueryable().Include(x => x.Children));
        }
        //todo: (feat) Add level property
        public async override Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            entity.Code = await GetNextChildCodeAsync(entity.ParentId);
            await ValidateEntityAsync(entity);
            return await base.InsertAsync(entity, autoSave, cancellationToken);
        }
        //todo: (feat) Add level property
        public async override Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await ValidateEntityAsync(entity);
            return await base.UpdateAsync(entity, autoSave, cancellationToken);
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
        
        
        
        public async Task MoveAsync(TEntity entity, Guid? parentId, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity.ParentId == parentId)
            {
                return;
            }

            //Should find children before Code change
            var children = await FindChildrenAsync(entity.Id, true);

            //Store old code of Tree
            var oldCode = entity.Code;

            //Move Tree
            entity.Code = await GetNextChildCodeAsync(parentId);
            entity.ParentId = parentId;

            await ValidateEntityAsync(entity);

            //Update Children Codes
            foreach (var child in children)
            {
                child.Code = TreeCodeDomainService.AppendCode(entity.Code, TreeCodeDomainService.GetRelativeCode(child.Code, oldCode));
                await base.UpdateAsync(child, autoSave, cancellationToken);
            }
            await base.UpdateAsync(entity, autoSave, cancellationToken);
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
                return TreeCodeDomainService.AppendCode(parentCode, TreeCodeDomainService.CreateCode(1));
            }

            return TreeCodeDomainService.CalculateNextCode(lastChild.Code);
        }
        protected virtual async Task<string> GetCodeAsync(Guid id)
        {
            return (await base.GetAsync(id)).Code;
        }
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
