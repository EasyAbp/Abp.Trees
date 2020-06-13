using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(TreesApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class TreesHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "Trees";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(TreesApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
