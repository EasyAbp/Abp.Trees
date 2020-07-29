using System;
using EasyAbp.Abp.Trees.EntityFrameworkCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpTreesServiceCollectionExtensions
    {
        public static IServiceCollection AddTreeRepository<TDbContext>(
            this IServiceCollection services)
            where TDbContext : AbpDbContext<TDbContext>
        {
            return AddTreeRepository(services, typeof(TDbContext));
        }
        public static IServiceCollection AddTreeRepository(
            this IServiceCollection services,Type dbContextType)
        {
            var options = new AbpTreesRepositoryRegistrationOptions(dbContextType, services);

            new EfCoreTreeRepositoryRegistrar(options).AddRepositories();

            return services;
        }
    }
}
