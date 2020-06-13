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
        Task<string> GetCodeAsync(Guid id);
        Task<TEntity> GetLastChildOrNullAsync(Guid? parentId);
        Task<string> GetNextChildCodeAsync(Guid? parentId);
        Task MoveAsync(TEntity entity, Guid? parentId, bool autoSave = false, CancellationToken cancellationToken = default);
    }
}