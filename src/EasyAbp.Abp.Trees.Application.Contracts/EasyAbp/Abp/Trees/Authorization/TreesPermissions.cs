using Volo.Abp.Reflection;

namespace EasyAbp.Abp.Trees.Authorization
{
    public class TreesPermissions
    {
        public const string GroupName = "Trees";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(TreesPermissions));
        }
    }
}