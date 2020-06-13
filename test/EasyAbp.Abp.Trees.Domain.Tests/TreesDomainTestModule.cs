using EasyAbp.Abp.Trees.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    /* Domain tests are configured to use the EF Core provider.
     * You can switch to MongoDB, however your domain tests should be
     * database independent anyway.
     */
    [DependsOn(
        typeof(TreesEntityFrameworkCoreTestModule)
        )]
    public class TreesDomainTestModule : AbpModule
    {
        
    }
}
