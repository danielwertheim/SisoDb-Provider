using System.Collections.Generic;
using NCore;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public class QueryIdentityItem : QueryXItem<int>
    {
        public static IList<QueryIdentityItem> CreateItems()
        {
            return new[]
            {
                new QueryIdentityItem{SortOrder = 1, StringValue = "A"},
                new QueryIdentityItem{SortOrder = 2, StringValue = "B"},
                new QueryIdentityItem{SortOrder = 3, StringValue = "C"},
                new QueryIdentityItem{SortOrder = 4, StringValue = "D"},
            };
        }

        public static IList<QueryIdentityItem> CreateFourUnorderedItems()
        {
            return new[]
            {
                new QueryIdentityItem{SortOrder = 2, StringValue = "B"},
                new QueryIdentityItem{SortOrder = 1, StringValue = "A"},
                new QueryIdentityItem{SortOrder = 4, StringValue = "D"},
                new QueryIdentityItem{SortOrder = 3, StringValue = "C"},
            };
        }

        public override string AsJson()
        {
            return JsonFormat.Inject(StructureId, SortOrder, StringValue);
        }
    }
}