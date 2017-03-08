using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TwentyTwenty.BaseLine;

namespace System.Linq
{
    public static class QuerableExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> queryable, SortSpec sortSpec)
        {
            var prop = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Single(p => p.Name.Equals(sortSpec.Member, StringComparison.OrdinalIgnoreCase));

            if (prop == null)
            {
                throw new ArgumentException($"No property '{sortSpec.Member}' in '{typeof(TEntity).Name}'");
            }

            var arg = Expression.Parameter(typeof(TEntity));
            var expr = Expression.Property(arg, prop);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), prop.PropertyType);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var methodName = sortSpec.SortDirection == ListSortDirection.Ascending ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods().Single(m => m.Name == methodName
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 2
                && m.GetParameters().Length == 2);

            return (IOrderedQueryable<TEntity>)method
                    .MakeGenericMethod(typeof(TEntity), prop.PropertyType)
                    .Invoke(null, new object[] { queryable, lambda });
        }

        public static PagedList<TEntity> ToPagedList<TEntity>(this IQueryable<TEntity> queryable, int pageNumber,
            int pageSize, SortSpec sortSpec = null)
        {
            if (sortSpec != null)
            {
                queryable = queryable.OrderBy(sortSpec);
            }

            return new PagedList<TEntity>(queryable, pageNumber, pageSize);
        }
    }
}