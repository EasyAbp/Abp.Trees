using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Abp.Trees
{
    public interface ITreeRepository<TEntity> : IRepository<TEntity, Guid>
        where TEntity : class, IEntity<Guid>, ITree<TEntity>
    {
        Task<List<TEntity>> FindChildrenAsync(Guid? parentId, bool recursive = false);
        Task MoveAsync(TEntity entity, Guid? parentId, bool autoSave = false, CancellationToken cancellationToken = default);
    }
}