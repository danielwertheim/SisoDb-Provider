using System.Collections.Generic;
using NCore;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public class QueryBigIdentityItem : QueryXItem<long>
    {
        public static IList<QueryBigIdentityItem> CreateItems()
        {
            return new[]
            {
                new QueryBigIdentityItem{SortOrder = 1, StringValue = "A"},
                new QueryBigIdentityItem{SortOrder = 2, StringValue = "B"},
                new QueryBigIdentityItem{SortOrder = 3, StringValue = "C"},
                new QueryBigIdentityItem{SortOrder = 4, StringValue = "D"},
            };
        }

        public static IList<QueryBigIdentityItem> CreateFourUnorderedItems()
        {
            return new[]
            {
                new QueryBigIdentityItem{SortOrder = 2, StringValue = "B"},
                new QueryBigIdentityItem{SortOrder = 1, StringValue = "A"},
                new QueryBigIdentityItem{SortOrder = 4, StringValue = "D"},
                new QueryBigIdentityItem{SortOrder = 3, StringValue = "C"},
            };
        }

        public override string AsJson()
        {
            return JsonFormat.Inject(StructureId, SortOrder, StringValue);
        }
    }
}