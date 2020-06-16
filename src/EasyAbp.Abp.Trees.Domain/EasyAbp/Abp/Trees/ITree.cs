using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Abp.Trees
{
    public interface ITree<TEntity>
        where TEntity : class, IEntity<Guid>
    {
        string Code { get; set; }
        int Level { get; set; }
        Guid? ParentId { get; set; }
        IList<TEntity> Children { get; set; }
        string DisplayName { get; set; }
    }
}
