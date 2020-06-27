using EasyAbp.Abp.Trees;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Abp.Trees.TestApp.Domain
{
    public class OrganizationUnit : AggregateRoot<Guid>, ITree<OrganizationUnit>
    {
        public OrganizationUnit() { }
        public OrganizationUnit(Guid id)
            : base(id)
        {
            Children = new List<OrganizationUnit>();
        }
        public virtual string DisplayName { get; set; }
        public virtual string Code { get; set; }
        public virtual int Level { get; set; }
        public virtual OrganizationUnit Parent { get; set; }
        public virtual Guid? ParentId { get; set; }
        public virtual ICollection<OrganizationUnit> Children { get; set; }
    }
}
