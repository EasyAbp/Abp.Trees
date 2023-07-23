using Volo.Abp.Domain;
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
        typeof(AbpValidationModule),
        typeof(AbpDddDomainSharedModule)
    )]
    public class AbpTreesDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpTreesDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<TreesResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/EasyAbp/Abp/Trees/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("EasyAbp.Abp.Trees", typeof(TreesResource));
            });
        }
    }
}
