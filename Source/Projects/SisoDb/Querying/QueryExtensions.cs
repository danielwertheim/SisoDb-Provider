namespace SisoDb.Querying
{
    public static class QueryExtensions
    {
        public static bool HasDependencies(this IQuery query)
        {
            return query.HasWhere || query.HasSortings;
        }
    }
}