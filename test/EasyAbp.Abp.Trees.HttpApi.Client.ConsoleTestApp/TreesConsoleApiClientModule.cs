using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(TreesHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class TreesConsoleApiClientModule : AbpModule
    {
        
    }
}
