using EasyAbp.Abp.Trees.TestApp.Application;
using System;
using System.Threading.Tasks;

namespace EasyAbp.Abp.Trees.TestApp
{
    public interface IOrganizationUnitAppService : Volo.Abp.Application.Services.ICrudAppService<Application.OrganizationUnitDto,
        Application.OrganizationUnitDto, Guid, Volo.Abp.Application.Dtos.IPagedAndSortedResultRequest,
        Application.CreateOrganizationUnitDto,Application.UpdateOrganizationUnitDto> { }
    public class OrganizationUnitAppService:
        Volo.Abp.Application.Services.CrudAppService<
            Domain.OrganizationUnit, Application.OrganizationUnitDto,
            Application.OrganizationUnitDto,Guid, Volo.Abp.Application.Dtos.IPagedAndSortedResultRequest,
            Application.CreateOrganizationUnitDto,Application.UpdateOrganizationUnitDto>,
        IOrganizationUnitAppService

    {
        public OrganizationUnitAppService(
            EasyAbp.Abp.Trees.ITreeRepository<Domain.OrganizationUnit> organizationUnitRepository
            ):base(organizationUnitRepository)
        {

        }

        // Mapperly cannot build an entity through its constructor and populate the nested
        // child tree, so the create/update input is applied to the entity explicitly here.
        protected override Task<Domain.OrganizationUnit> MapToEntityAsync(CreateOrganizationUnitDto createInput)
        {
            return Task.FromResult(BuildEntity(createInput));
        }

        protected override Task MapToEntityAsync(UpdateOrganizationUnitDto updateInput, Domain.OrganizationUnit entity)
        {
            entity.DisplayName = updateInput.DisplayName;
            entity.Code = updateInput.Code;
            entity.Level = updateInput.Level;
            entity.ParentId = updateInput.ParentId;
            return Task.CompletedTask;
        }

        private static Domain.OrganizationUnit BuildEntity(CreateOrganizationUnitDto input)
        {
            var entity = new Domain.OrganizationUnit(input.Id)
            {
                DisplayName = input.DisplayName,
                Code = input.Code,
                Level = input.Level,
                ParentId = input.ParentId
            };

            if (input.Children != null)
            {
                foreach (var child in input.Children)
                {
                    entity.Children.Add(BuildEntity(child));
                }
            }

            return entity;
        }
    }
}
