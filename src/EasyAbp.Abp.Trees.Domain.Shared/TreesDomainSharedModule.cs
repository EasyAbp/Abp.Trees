using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;

namespace EasyAbp.Abp.Trees
{
    [DependsOn(
        typeof(AbpValidationModule)
    )]
    public class TreesDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<TreesDomainSharedModule>("EasyAbp.Abp.Trees");
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<TreesResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/Trees");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("EasyAbp.Abp.Trees", typeof(TreesResource));
            });
        }
    }
}
