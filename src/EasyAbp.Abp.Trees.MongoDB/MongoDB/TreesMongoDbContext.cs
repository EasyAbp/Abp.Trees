using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.Trees.MongoDB
{
    [ConnectionStringName(TreesDbProperties.ConnectionStringName)]
    public class TreesMongoDbContext : AbpMongoDbContext, ITreesMongoDbContext
    {
        /* Add mongo collections here. Example:
         * public IMongoCollection<Question> Questions => Collection<Question>();
         */

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureTrees();
        }
    }
}