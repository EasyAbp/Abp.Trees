using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(TreesApplicationModule),
        typeof(TreesDomainTestModule)
        )]
    public class TreesApplicationTestModule : AbpModule
    {

    }
}
