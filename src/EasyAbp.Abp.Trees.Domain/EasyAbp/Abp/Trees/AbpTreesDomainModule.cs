using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpTreesDomainSharedModule)
        )]
    public class AbpTreesDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.TryAddTransient(typeof(ITreeCodeGenerator<>), typeof(TreeCodeGenerator<>));
        }
    }
}
