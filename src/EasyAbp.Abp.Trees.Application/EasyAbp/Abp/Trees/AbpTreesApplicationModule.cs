using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpTreesDomainModule),
        typeof(AbpTreesApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class AbpTreesApplicationModule : AbpModule
    {
    }
}
