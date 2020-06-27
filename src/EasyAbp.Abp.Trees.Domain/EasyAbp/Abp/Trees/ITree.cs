using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
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
    public static class ITreeExtensions
    {
        public static void SetCode<TEntity>(this ITree<TEntity> entity, string code)
            where TEntity : class, ITree<TEntity>
        {
            entity.Code = code;
            entity.Level = entity.Code.Split('.').Length;
        }
        public static void MoveTo<TEntity>(this ITree<TEntity> entity, TEntity parent)
            where TEntity : class, ITree<TEntity>
        {
            parent.Children.Add(entity as TEntity);
            if (entity.Parent != null)
            {
                entity.Parent.Children.Remove(entity as TEntity);
            }

            entity.Parent = parent;
            entity.ParentId = parent.Id;

        }
    }
}
