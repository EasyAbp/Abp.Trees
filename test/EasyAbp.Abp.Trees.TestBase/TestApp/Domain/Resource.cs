using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Abp.Trees.TestApp.Domain
{
    public class Resource : AggregateRoot<Guid>, ITree<Resource>
    {
        public Resource(Guid id)
            : base(id)
        {
            Children = new List<Resource>();
        }
        public string DisplayName { get; set; }
        public virtual string Code { get; set; }
        public virtual int Level { get; set; }
        public virtual Resource Parent { get; set; }
        public virtual Guid? ParentId { get; set; }
        public virtual ICollection<Resource> Children { get; set; }
        public virtual string Path { get; set; }
    }
}
