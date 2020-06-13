using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpTreesApplicationModule),
        typeof(TreesDomainTestModule)
        )]
    public class TreesApplicationTestModule : AbpModule
    {

    }
}
