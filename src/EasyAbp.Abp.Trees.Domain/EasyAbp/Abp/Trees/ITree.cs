using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Abp.Trees
{
    public interface ITree<TEntity> : IEntity<Guid> 
        where TEntity : class, ITree<TEntity>
    {
        string Code { get; set; }
        int Level { get; set; }
        Guid? ParentId { get; set; }
        TEntity Parent { get; set; }
        ICollection<TEntity> Children { get; set; }
        string DisplayName { get; set; }
    }
}
