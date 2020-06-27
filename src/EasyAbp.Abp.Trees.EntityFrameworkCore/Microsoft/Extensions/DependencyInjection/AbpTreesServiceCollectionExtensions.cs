using EasyAbp.Abp.Trees.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpTreesServiceCollectionExtensions
    {
        public static IServiceCollection AddTreeRepository<TDbContext>(
            this IServiceCollection services)
            where TDbContext : AbpDbContext<TDbContext>
        {
            var options = new AbpTreesRepositoryRegistrationOptions(typeof(TDbContext), services);

            //TODO: Custom option action
            //optionsBuilder?.Invoke(options);

            new EfCoreTreeRepositoryRegister(options).AddRepositories();

            return services;
        }
    }
}
