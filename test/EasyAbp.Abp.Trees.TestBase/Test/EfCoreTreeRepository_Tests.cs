using EasyAbp.Abp.Trees.TestApp.Domain;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Xunit;

namespace EasyAbp.Abp.Trees.Test
{
    /* Write your custom repository tests like that, in this project, as abstract classes.
     * Then inherit these abstract classes from EF Core & MongoDB test projects.
     * In this way, both database providers are tests with the same set tests.
     */
    public abstract class EfCoreTreeRepository_Tests<TStartupModule> : TreesTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ITreeRepository<OrganizationUnit> _organizationUnitRepository;
        private readonly ITreeRepository<Resource> _resourceRepository;
        private readonly IGuidGenerator _guidGenerator;
        protected EfCoreTreeRepository_Tests()
        {
            _organizationUnitRepository = GetRequiredService<ITreeRepository<OrganizationUnit>>();
            _resourceRepository = GetRequiredService<ITreeRepository<Resource>>();
            _guidGenerator = GetRequiredService<IGuidGenerator>();

        }

        private void addChildren(OrganizationUnit node, int count)
        {

            Enumerable.Range(1, count)
                 .ToList()
                 .Select(x =>
                 {
                     var id = _guidGenerator.Create();
                     return new OrganizationUnit(id) { ParentId = node.Id, Parent = node, DisplayName = node.DisplayName + "-Child-" + x };
                 })
                 .ToList()
                 .ForEach(x => node.Children.Add(x));
        }
        private OrganizationUnit createTestData()
        {
            var rootId = _guidGenerator.Create();
            var root = new OrganizationUnit(rootId) { DisplayName = "Root" };
            addChildren(root, 10);
            root.Children.ToList()
                .ForEach(c =>
                {
                    addChildren(c, 5);
                });
            return root;
        }

        [Fact]
        public async Task InsertWithChildTestAsync()
        {
            var root = createTestData();

            await _organizationUnitRepository.InsertAsync(root, true);
            var list = (await _organizationUnitRepository.GetListAsync()).OrderBy(x => x.Code).ToList();
            list.Count.ShouldBe(61);
            var afterInsertedRoot = list.Single(x => x.Id == root.Id);
            afterInsertedRoot.ShouldNotBeNull();
        }
        [Fact]
        public async Task InsertBigCodeLengthTestAsync()
        {
            var rootId = _guidGenerator.Create();
            var root = new Resource(rootId) { DisplayName = "Root" };

            await _resourceRepository.InsertAsync(root, true);
            var list = (await _resourceRepository.GetListAsync()).OrderBy(x => x.Code).ToList();
            var afterInsertedRoot = list.Single(x => x.Id == root.Id);
            afterInsertedRoot.Code.Length.ShouldBe(10);
        }
        [Fact]
        public async Task InsertTwoRootsTestAsync()
        {
            var firstRoot = createTestData();
            var secondRoot = createTestData();

            await _organizationUnitRepository.InsertAsync(firstRoot, true);
            await _organizationUnitRepository.InsertAsync(secondRoot, true);

            var list = (await _organizationUnitRepository.GetListAsync()).OrderBy(x => x.Code).ToList();

            var roots = list.Where(x => x.ParentId == null).ToList();
            roots.Count().ShouldBe(2);
            roots[0].Code.ShouldNotBe(roots[1].Code);
        }

        [Fact]
        public async Task UpdateWithMoveTestAsync()
        {
            await WithUnitOfWorkAsync(async () =>
                 {
                     var root = createTestData();
                     await _organizationUnitRepository.InsertAsync(root, true);

                     var afterInsertedRoot = (await _organizationUnitRepository.GetListAsync()).OrderBy(x => x.Code).ToList().Single(x => x.Id == root.Id);

                     var source = afterInsertedRoot.Children.FirstOrDefault();
                     var target = afterInsertedRoot.Children.LastOrDefault().Children.FirstOrDefault();
                     source.MoveTo(target);


                     await _organizationUnitRepository.UpdateAsync(source, true);
                     var afterUpdatedRoot = (await _organizationUnitRepository.GetListAsync()).OrderBy(x => x.Code).ToList().Single(x => x.Id == root.Id);


                     afterUpdatedRoot.Children.Count.ShouldBe(9);
                     afterUpdatedRoot.Children.LastOrDefault().Children.FirstOrDefault().Children.Count.ShouldBe(1);

                 });
        }

        [Fact]
        public async Task DeleteTestAsync()
        {
            var firstRoot = createTestData();
            await _organizationUnitRepository.InsertAsync(firstRoot, true);


            var toDeleteNode = firstRoot.Children.FirstOrDefault();

            await _organizationUnitRepository.DeleteAsync(toDeleteNode);

            var list = (await _organizationUnitRepository.GetListAsync()).OrderBy(x => x.Code).ToList();
            list.Count().ShouldBe(55);
        }


    }
}
