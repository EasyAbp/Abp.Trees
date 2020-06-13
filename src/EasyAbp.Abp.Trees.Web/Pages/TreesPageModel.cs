using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.Abp.Trees.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class TreesPageModel : AbpPageModel
    {
        protected TreesPageModel()
        {
            LocalizationResourceType = typeof(TreesResource);
        }
    }
}