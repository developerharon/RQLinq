namespace RQLinq.Extensions
{
    public static class EnumerableExtensions
    {
        public static IQueryable<T> WhereRql<T>(this IQueryable<T> list, string rql)
        {
            var func = RqlEvaluator.Evaluate<T>(rql);
            return list.Where(func);
        }
    }
}