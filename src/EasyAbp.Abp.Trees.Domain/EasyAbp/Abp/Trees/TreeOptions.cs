using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace EasyAbp.Abp.Trees
{
    public class TreeOptions<TEntity>
        where TEntity : class, ITree<TEntity>
    {
        public static TreeOptions<TEntity> Empty { get; } = new TreeOptions<TEntity>() { CodeLength = 5 };
        public int CodeLength { get; set; }
    }

    public class TreeOptions
    {
        private readonly IDictionary<Type, object> _options;

        public TreeOptions()
        {
            _options = new Dictionary<Type, object>();
        }
        public TreeOptions<TEntity> GetOrNull<TEntity>()
            where TEntity : class, ITree<TEntity>
        {
            return _options.GetOrDefault(typeof(TEntity)) as TreeOptions<TEntity>;
        }

        public void TreeEntity<TEntity>([NotNull] Action<TreeOptions<TEntity>> optionsAction)
            where TEntity : class, ITree<TEntity>
        {
            Check.NotNull(optionsAction, nameof(optionsAction));

            optionsAction(
                _options.GetOrAdd(
                    typeof(TEntity),
                    () => new TreeOptions<TEntity>()
                ) as TreeOptions<TEntity>
            );
        }

    }
}
