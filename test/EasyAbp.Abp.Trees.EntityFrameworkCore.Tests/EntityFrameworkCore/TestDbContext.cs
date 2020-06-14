using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore
{
    public class TestDbContext : AbpDbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}