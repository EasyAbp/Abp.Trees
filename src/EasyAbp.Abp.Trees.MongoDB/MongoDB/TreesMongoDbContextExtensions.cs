using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.Trees.MongoDB
{
    public static class TreesMongoDbContextExtensions
    {
        public static void ConfigureTrees(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new TreesMongoModelBuilderConfigurationOptions(
                TreesDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);
        }
    }
}