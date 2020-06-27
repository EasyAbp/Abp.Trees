namespace EasyAbp.Abp.Trees
{
    public static class TreeExtensions
    {
        public static void SetCode<TEntity>(this ITree<TEntity> entity, string code)
            where TEntity : class, ITree<TEntity>
        {
            entity.Code = code;
            entity.Level = entity.Code.Split('.').Length;
        }

        public static void MoveTo<TEntity>(this ITree<TEntity> entity, TEntity parent)
            where TEntity : class, ITree<TEntity>
        {
            parent.Children.Add(entity as TEntity);
            entity.Parent?.Children.Remove(entity as TEntity);

            entity.Parent = parent;
            entity.ParentId = parent.Id;
        }
    }
}