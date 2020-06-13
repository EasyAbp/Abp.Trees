using EasyAbp.Abp.Trees.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.Abp.Trees.Authorization
{
    public class TreesPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            //var moduleGroup = context.AddGroup(TreesPermissions.GroupName, L("Permission:Trees"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<TreesResource>(name);
        }
    }
}