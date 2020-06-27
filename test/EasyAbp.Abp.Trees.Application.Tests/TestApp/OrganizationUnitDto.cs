using EasyAbp.Abp.Trees;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Abp.Trees.TestApp.Application
{
    public class OrganizationUnitDto : Volo.Abp.Application.Dtos.EntityDto<Guid>
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
    }

    public class CreateOrganizationUnitDto : Volo.Abp.Application.Dtos.EntityDto<Guid>
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public List<CreateOrganizationUnitDto> Children { get; set; }
    }
    public class UpdateOrganizationUnitDto
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
    }
}
