using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Reflection;

namespace EasyAbp.Abp.Trees.EntityFrameworkCore.DependencyInjection
{
    public class EfCoreTreeRepositoryRegistrar //: RepositoryRegistrarBase<AbpDbContextRegistrationOptions>
    {
        public AbpTreesRepositoryRegistrationOptions Options { get; }
        public EfCoreTreeRepositoryRegistrar(AbpTreesRepositoryRegistrationOptions options)
        {
            Options = options;
        }
        public void AddRepositories()
        {
            RegisterDefaultRepositories();
        }
        protected virtual void RegisterDefaultRepositories()
        {
            foreach (var entityType in GetEntityTypes(Options.OriginalDbContextType))
            {
                if (!ShouldRegisterDefaultRepositoryFor(entityType))
                {
                    continue;
                }

                RegisterDefaultRepository(entityType);
            }
        }
        protected void RegisterDefaultRepository(Type entityType)
        {
            var repositoryImplementationType = GetDefaultRepositoryImplementationType(entityType);

            var treeRepositoryInterface = typeof(ITreeRepository<>).MakeGenericType(entityType);
            if (treeRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                Options.Services.TryAddTransient(treeRepositoryInterface, repositoryImplementationType);
            }
        }

        protected Type GetDefaultRepositoryImplementationType(Type entityType)
        {
            return GetRepositoryType(Options.DefaultRepositoryDbContextType, entityType);
        }
        protected IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];
        }

        protected Type GetRepositoryType(Type dbContextType, Type entityType)
        {
            return typeof(EfCoreTreeRepository<,>).MakeGenericType(dbContextType, entityType);
        }

        protected bool ShouldRegisterDefaultRepositoryFor(Type entityType)
        {
            var isTreeEntity = entityType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(EasyAbp.Abp.Trees.ITree<>));

            return isTreeEntity;
        }
    }
}
