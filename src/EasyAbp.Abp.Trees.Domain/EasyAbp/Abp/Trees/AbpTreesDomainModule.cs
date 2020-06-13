using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpTreesDomainSharedModule)
        )]
    public class AbpTreesDomainModule : AbpModule
    {

    }
}
