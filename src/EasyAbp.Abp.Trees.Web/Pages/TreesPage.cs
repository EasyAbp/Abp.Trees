using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.Abp.Trees.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits EasyAbp.Abp.Trees.Web.Pages.TreesPage
     */
    public abstract class TreesPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<TreesResource> L { get; set; }
    }
}
