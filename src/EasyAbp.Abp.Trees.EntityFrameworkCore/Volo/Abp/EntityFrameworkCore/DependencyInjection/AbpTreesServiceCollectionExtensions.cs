using EasyAbp.Abp.Trees;
using EasyAbp.Abp.Trees.EntityFrameworkCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.EntityFrameworkCore.DependencyInjection
{
    public static class IAbpDbContextRegistrationOptionsBuilderExtensions
    {
        public static void TreeEntity<TEntity>(this IAbpDbContextRegistrationOptionsBuilder builder, Action<TreeOptions<TEntity>> optionsAction)
            where TEntity : class, ITree<TEntity>
        {
            builder.Services.Configure<TreeOptions>(options =>
            {
                options.TreeEntity(optionsAction);
            });
        }
        public static void AddDefaultTreeRepositories(this IAbpDbContextRegistrationOptionsBuilder builder)
        {
            var realOptions = (builder as AbpDbContextRegistrationOptions);

            builder.Services.AddTreeRepository(realOptions.OriginalDbContextType);
        }

    }
}
