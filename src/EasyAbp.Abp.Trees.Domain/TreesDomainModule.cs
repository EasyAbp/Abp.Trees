using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(TreesDomainSharedModule)
        )]
    public class TreesDomainModule : AbpModule
    {

    }
}
