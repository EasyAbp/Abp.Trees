using EasyAbp.Abp.Trees.TestApp;
using EasyAbp.Abp.Trees.TestApp.Application;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyAbp.Abp.Trees.Test
{
    public class OrganizationUnitAppServiceTest : TreesApplicationTestBase
    {
        private readonly IOrganizationUnitAppService _crudAppService;
        public OrganizationUnitAppServiceTest()
        {
            _crudAppService = GetRequiredService<IOrganizationUnitAppService>();
        }

        private void addChildren(CreateOrganizationUnitDto node, int count)
        {

            Enumerable.Range(1, count)
                 .ToList()
                 .Select(x =>
                 {
                     return new CreateOrganizationUnitDto() { DisplayName = node.DisplayName + "-Child-" + x, Children = new List<CreateOrganizationUnitDto>() };
                 })
                 .ToList()
                 .ForEach(x => node.Children.Add(x));
        }
        private CreateOrganizationUnitDto createTestData()
        {
            //var rootId = _guidGenerator.Create();
            var root = new CreateOrganizationUnitDto() { DisplayName = "Root", Children = new List<CreateOrganizationUnitDto>() };
            addChildren(root, 10);
            root.Children.ToList()
                .ForEach(c =>
                {
                    addChildren(c, 5);
                });
            return root;
        }


        [Fact]
        public async Task InsertWithChildAsync()
        {
            var root = createTestData();

            await _crudAppService.CreateAsync(root);


            var listAfterInserted = await _crudAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto()
            {
                MaxResultCount = 100,
                SkipCount = 0,
                Sorting = ""
            });
            listAfterInserted.Items.Count().ShouldBe(61);
        }
        [Fact]
        public async Task UpdateWithMoveAsync()
        {
            var root = createTestData();
            await _crudAppService.CreateAsync(root);
            var listAfterCreated = await _crudAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto() { MaxResultCount = 1000 });
            var source = listAfterCreated.Items.Single(x => x.Code == "00001.00001");
            var sourceName = source.DisplayName;
            var target = listAfterCreated.Items.Single(x => x.Code == "00001.00010.00001");
            var targetName = target.DisplayName;
            var updateDto = new UpdateOrganizationUnitDto()
            {
                Code = source.Code,
                Level = source.Level,
                DisplayName = source.DisplayName,
                ParentId = target.Id
            };
            await _crudAppService.UpdateAsync(source.Id, updateDto);

            var listAfterUpdated = await _crudAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto() { MaxResultCount = 1000 });
            var toCheckSource = listAfterUpdated.Items.Single(x => x.DisplayName == sourceName);
            var toCheckTarget = listAfterUpdated.Items.Single(x => x.DisplayName == targetName);
            toCheckSource.ParentId.ShouldBe(toCheckTarget.Id);
            toCheckSource.Level.ShouldBe(4);

            var toCheckSourceChildren = listAfterUpdated.Items.Where(x => x.ParentId == toCheckSource.Id);
            toCheckSourceChildren.All(x => x.Level == 5).ShouldBeTrue();
        }
    }
}
