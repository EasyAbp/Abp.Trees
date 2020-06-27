using EasyAbp.Abp.Trees.TestApp.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

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

    }
}
