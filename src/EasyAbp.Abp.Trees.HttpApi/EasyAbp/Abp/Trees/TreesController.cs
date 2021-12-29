using EasyAbp.Abp.Trees.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Abp.Trees
{
    [Area(AbpTreesRemoteServiceConsts.ModuleName)]
    public abstract class TreesController : AbpControllerBase
    {
        protected TreesController()
        {
            LocalizationResource = typeof(TreesResource);
        }
    }
}
