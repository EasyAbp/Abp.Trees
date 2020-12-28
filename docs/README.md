# Abp.Trees

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FAbpVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FEasyAbp%2FAbp.Trees%2Fmaster%2FDirectory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.Trees.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.Trees.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.Trees.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.Trees.Domain.Shared)
[![GitHub stars](https://img.shields.io/github/stars/EasyAbp/Abp.Trees?style=social)](https://www.github.com/EasyAbp/Abp.Trees)

An abp module that provides standard tree structure entity implement.

## Installation

1. Install the following NuGet packages. ([see how](https://github.com/EasyAbp/EasyAbpGuide/blob/master/How-To.md#add-nuget-packages))

    * EasyAbp.Abp.Trees.Domain
    * EasyAbp.Abp.Trees.Domain.Shared
    * EasyAbp.Abp.Trees.EntityFrameworkCore

## Usage

1. Create a entity and implement `ITree<TEntity>`.

1. Create a Repository for the entity.
	`EfCoreTreeRepository<TDbContext, TEntity>` override some function of `EfCoreRepository<TDbContext, TEntity, TKey>` to match tree structure:

	* `InsertAsync` :Auto Append node `Code` and Calc `Level` property when insert

	* `UpdateAsync` :Auto Move node when update a `Entity` that parentId is modified

	* `DeleteAsync` :Also delete `Children` nodes 

1. You have two ways to use this `Repository`

	* Way 1 : Default Repository(`ITreeRepository<>`),  
	  Add `context.Services.AddTreeRepository<MyProjectNameDbContext>();` to ConfigureServices method in `MyProjectNameEntityFrameworkCoreModule.cs`.

	* Way 2 : Create a `CustomRepository` that base on `EfCoreTreeRepository<TDbContext, TEntity>`

	* Example:
	```csharp
	context.Services.AddAbpDbContext<TestDbContext>(options =>
	{
		options.AddDefaultRepositories(includeAllEntities: true);//add Abp's `IRepository<TEntity>`
		options.AddDefaultTreeRepositories();//add `ITreeRepository<TEntity>` for all Entity with implement `ITree<TEntity>`
		options.TreeEntity<Resource>(x => x.CodeLength = 10);//set CodeLength for each Entity(Default:5)
	});
	```

## Sample

It works fine with `Volo.Abp.Application.Services.CrudAppService`.

After replacing `IRepository<>` with `ITreeRepository<Domain.OrganizationUnit>`, the repository will handle the tree structure of the entity during creating, updating, and deleting.

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
