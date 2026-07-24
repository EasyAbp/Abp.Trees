using EasyAbp.Abp.Trees.TestApp.Domain;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace EasyAbp.Abp.Trees.TestApp.Application
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class OrganizationUnitToOrganizationUnitDtoMapper : MapperBase<OrganizationUnit, OrganizationUnitDto>
    {
        public override partial OrganizationUnitDto Map(OrganizationUnit source);

        public override partial void Map(OrganizationUnit source, OrganizationUnitDto destination);
    }
}
