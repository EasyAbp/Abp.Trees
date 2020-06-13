using Localization.Resources.AbpUi;
using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpTreesApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class AbpTreesHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpTreesHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<TreesResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
