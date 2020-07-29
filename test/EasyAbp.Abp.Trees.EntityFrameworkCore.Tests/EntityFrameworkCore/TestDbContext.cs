using EasyAbp.Abp.Trees.TestApp.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore
{
    public class TestDbContext : AbpDbContext<TestDbContext>
    {
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public TestDbContext(DbContextOptions<TestDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrganizationUnit>(b =>
            {
                b.TryConfigureExtraProperties();
                b.TryConfigureConcurrencyStamp();
            });
            builder.Entity<Resource>(b =>
            {
                b.TryConfigureExtraProperties();
                b.TryConfigureConcurrencyStamp();
            });
        }
    }
}