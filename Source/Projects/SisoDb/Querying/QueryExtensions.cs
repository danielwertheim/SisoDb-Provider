namespace SisoDb.Querying
{
    public static class QueryExtensions
    {
        public static bool HasNoDependencies(this IQuery query)
        {
            return query.IsEmpty || (query.HasTakeNumOfStructures && !query.HasPaging && !query.HasSkipNumOfStructures && !query.HasSortings && !query.HasWhere);
        }
    }
}