using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.Trees.MongoDB
{
    [ConnectionStringName(TreesDbProperties.ConnectionStringName)]
    public interface ITreesMongoDbContext : IAbpMongoDbContext
    {
        /* Define mongo collections here. Example:
         * IMongoCollection<Question> Questions { get; }
         */
    }
}
