using EasyAbp.Abp.Trees.TestApp.Domain;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EasyAbp.Abp.Trees.Samples
{
    public class SampleManager_Tests : TreesDomainTestBase
    {
        private readonly IRepository<OrganizationUnit> _treeRepository;
        public SampleManager_Tests()
        {
            _treeRepository= GetRequiredService<IRepository<OrganizationUnit>>();
        }

        //[Fact]
        //public async Task Method1Async()
        //{
        //    await WithUnitOfWorkAsync(() =>
        //    {
        //        var t = _treeRepository.ToList();
        //    });
            
        //}
    }
}
