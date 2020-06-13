namespace EasyAbp.Abp.Trees
{
    public static class TreesDbProperties
    {
        public static string DbTablePrefix { get; set; } = "Trees";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "Trees";
    }
}
