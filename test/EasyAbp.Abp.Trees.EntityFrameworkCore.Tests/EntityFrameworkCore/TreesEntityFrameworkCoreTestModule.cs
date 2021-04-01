using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using EasyAbp.Abp.Trees.TestApp.Domain;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore
{
    [DependsOn(
        typeof(TreesTestBaseModule),
        typeof(AbpTreesEntityFrameworkCoreModule)
        )]
    public class TreesEntityFrameworkCoreTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTreeRepository<TestDbContext>();

            context.Services.AddAbpDbContext<TestDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
                //options.AddDefaultRepositories<TestDbContext>().SetDefaultRepositoryClasses()
                //options.AddDefaultTreeRepositories();
                options.TreeEntity<Resource>(x => x.CodeLength = 10);
            });

            var sqliteConnection = CreateDatabaseAndGetConnection();

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
                });
            });
        }
        
        private static SqliteConnection CreateDatabaseAndGetConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            new TestDbContext(
                new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options
            ).GetService<IRelationalDatabaseCreator>().CreateTables();
            
            return connection;
        }
    }
}
