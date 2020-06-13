using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.Application.Services;

namespace EasyAbp.Abp.Trees
{
    public abstract class TreesAppService : ApplicationService
    {
        protected TreesAppService()
        {
            LocalizationResource = typeof(TreesResource);
        }
    }
}
