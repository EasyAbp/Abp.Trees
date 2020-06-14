using EasyAbp.Abp.Trees.EntityFrameworkCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpTreesDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class AbpTreesEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}