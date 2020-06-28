using EasyAbp.Abp.Trees.TestApp.Application;
using EasyAbp.Abp.Trees.TestApp.Domain;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpTreesApplicationModule),
        typeof(TreesDomainTestModule)
        )]
    public class TreesApplicationTestModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ConfigureAutoMapper();
            //ConfigureDistributedEventBus();
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.Configurators.Add(ctx =>
                {
                    ctx.MapperConfiguration.CreateMap<OrganizationUnit, OrganizationUnitDto>().ReverseMap();
                    ctx.MapperConfiguration.CreateMap<OrganizationUnit, CreateOrganizationUnitDto>().ReverseMap();
                    ctx.MapperConfiguration.CreateMap<OrganizationUnit, UpdateOrganizationUnitDto>().ReverseMap();
                });

                options.AddMaps<TreesApplicationTestModule>();
            });
        }

    }
}
