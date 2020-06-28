using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore.DependencyInjection
{
    public class AbpTreesRepositoryRegistrationOptions
    {
        public Type OriginalDbContextType { get; }

        public Type DefaultRepositoryDbContextType { get; protected set; }

        public IServiceCollection Services { get; }

        public AbpTreesRepositoryRegistrationOptions(Type originalDbContextType, IServiceCollection services)
        {
            OriginalDbContextType = originalDbContextType;
            DefaultRepositoryDbContextType = originalDbContextType;
            Services = services;
        }
    }
}
