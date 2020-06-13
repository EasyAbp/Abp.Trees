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
            var options = new AbpTreesRepositoryRegistrationOptions(typeof(TDbContext), services);

            //TODO: Custom option action
            //optionsBuilder?.Invoke(options);

            new EfCoreTreeRepositoryRegistrar(options).AddRepositories();

            return services;
        }
    }
}
