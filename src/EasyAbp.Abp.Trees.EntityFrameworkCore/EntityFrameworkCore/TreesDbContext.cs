using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore
{
    [ConnectionStringName(TreesDbProperties.ConnectionStringName)]
    public class TreesDbContext : AbpDbContext<TreesDbContext>, ITreesDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public TreesDbContext(DbContextOptions<TreesDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureTrees();
        }
    }
}