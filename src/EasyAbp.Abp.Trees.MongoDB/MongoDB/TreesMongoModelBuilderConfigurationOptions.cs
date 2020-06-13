using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.Trees.MongoDB
{
    public class TreesMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public TreesMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}