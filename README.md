# Abp.Trees

[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.Trees.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.Trees.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.Trees.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.Trees.Domain.Shared)

An abp module that provides standard tree structure entity implement.

## Getting Started

* Install with [AbpHelper](https://github.com/EasyAbp/AbpHelper.GUI)

    Coming soon.

* Install Manually

    1. Install `EasyAbp.Abp.Trees.Domain` NuGet package to `MyProject.Domain` project and add `[DependsOn(AbpTreesDomainModule)]` attribute to the module.

    1. Install `EasyAbp.Abp.Trees.Domain.Shared` NuGet package to `MyProject.Domain.Shared` project and add `[DependsOn(AbpTreesDomainSharedModule)]` attribute to the module.

    1. Install `EasyAbp.Abp.Trees.EntityFrameworkCore` NuGet package to `MyProject.EntityFrameworkCore` project and add `[DependsOn(AbpTreesEntityFrameworkCoreModule)]` attribute to the module.    

## Usage

1. Create a entity and implement `ITree<TEntity>`.

1. Repository: 
`EfCoreTreeRepository<TDbContext, TEntity>` override some function of `EfCoreRepository<TDbContext, TEntity, TKey>` to match tree structure:

* `InsertAsync` :Auto Append node `Code` and Calc `Level` property when insert

* `UpdateAsync` :Auto Move node when update a `Entity` that parentId is modified

* `DeleteAsync` :Also delete `Children` nodes 

You have two ways to use this `Repository`

* Way 1 : Default Repository(`ITreeRepository<>`),  
  Add `context.Services.AddTreeRepository<MyProjectNameDbContext>();` to ConfigureServices method in `MyProjectNameEntityFrameworkCoreModule.cs`.
* Way 2 : Create a `CustomRepository` that base on `EfCoreTreeRepository<TDbContext, TEntity>`

* Example:
```charp
            context.Services.AddAbpDbContext<TestDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);//add Abp's `IRepository<TEntity>`
                options.AddDefaultTreeRepositories();//add `ITreeRepository<TEntity>` for all Entity with implement `ITree<TEntity>`
                options.TreeEntity<Resource>(x => x.CodeLength = 10);//set CodeLength for each Entity(Default:5)
            });
```

## Sample
It works fine with `Volo.Abp.Application.Services.CrudAppService`.
Just like `strategy pattern` to Replace `IRepository<>` with `ITreeRepository<Domain.OrganizationUnit>`,
It will handle tree structure of entity when Create,Update,Delete

```csharp
    public class OrganizationUnitAppService:
        Volo.Abp.Application.Services.CrudAppService<
            Domain.OrganizationUnit, Application.OrganizationUnitDto,
            Application.OrganizationUnitDto,Guid, Volo.Abp.Application.Dtos.IPagedAndSortedResultRequest,
            Application.CreateOrganizationUnitDto,Application.UpdateOrganizationUnitDto>,
        IOrganizationUnitAppService
        
    {
        public OrganizationUnitAppService(
            EasyAbp.Abp.Trees.ITreeRepository<Domain.OrganizationUnit> organizationUnitRepository
            ):base(organizationUnitRepository)
        {
            
        }

    }
```

## Roadmap

- [ ] Widget of tree operation for MVC UI.
- [ ] Create a TreeManager to provides more function,example: `Sort`(reassigned code),` Ui Pagination`...
- [ ] More Unit tests.
