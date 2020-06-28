using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Abp.Trees
{
    public interface ITreeRepository<TEntity> : IRepository<TEntity, Guid>
        where TEntity : class, IEntity<Guid>, ITree<TEntity>
    {
        Task<List<TEntity>> GetChildrenAsync(Guid? parentId, bool includeDetails = true, bool recursive = false, CancellationToken cancellationToken = default);
    }
}