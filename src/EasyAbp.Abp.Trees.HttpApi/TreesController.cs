using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Abp.Trees
{
    public abstract class TreesController : AbpController
    {
        protected TreesController()
        {
            LocalizationResource = typeof(TreesResource);
        }
    }
}
