using EasyAbp.Abp.Trees.TestApp.Domain;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EasyAbp.Abp.Trees.Samples
{
    public class TreeRepository_Tests : TreesDomainTestBase
    {
        private readonly ITreeRepository<OrganizationUnit> _treeRepository;
        public TreeRepository_Tests()
        {
            _treeRepository = GetRequiredService<ITreeRepository<OrganizationUnit>>();
        }

        [Fact]
        public async Task InsertWithoutSaveTestAsync()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                List<OrganizationUnit> testDatas = new List<OrganizationUnit>()
                {
                    new OrganizationUnit(){DisplayName="1" },
                    new OrganizationUnit(){DisplayName="2" }
                };
                foreach (var d in testDatas)
                {
                    await _treeRepository.InsertAsync(d);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var query = await _treeRepository.GetQueryableAsync();

                query.GroupBy(x => x.Code).Count().ShouldBe(2);
            });

        }

        [Fact]
        public async Task InsertManyTestAsync()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                List<OrganizationUnit> testDatas = new List<OrganizationUnit>()
                {
                    new OrganizationUnit(){DisplayName="1" },
                    new OrganizationUnit(){DisplayName="2" }
                };

                await _treeRepository.InsertManyAsync(testDatas);

            });

            await WithUnitOfWorkAsync(async () =>
            {
                var query = await _treeRepository.GetQueryableAsync();
                var datas = query.ToList();

                datas.GroupBy(x => x.Code).Count().ShouldBe(2);
            });

        }
    }
}
