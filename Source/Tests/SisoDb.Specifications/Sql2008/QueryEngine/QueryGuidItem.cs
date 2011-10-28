using System;
using System.Collections.Generic;
using NCore;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public class QueryGuidItem : QueryXItem<Guid>
    {
        public static IList<QueryGuidItem> CreateFourItems()
        {
            return new[]
            {
                new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                new QueryGuidItem{SortOrder = 2, StringValue = "B"},
                new QueryGuidItem{SortOrder = 3, StringValue = "C"},
                new QueryGuidItem{SortOrder = 4, StringValue = "D"},
            };
        }

        public static IList<QueryGuidItem> CreateFourUnorderedItems()
        {
            return new[]
            {
                new QueryGuidItem{SortOrder = 2, StringValue = "B"},
                new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                new QueryGuidItem{SortOrder = 4, StringValue = "D"},
                new QueryGuidItem{SortOrder = 3, StringValue = "C"},
            };
        }

        public override string AsJson()
        {
            return JsonFormat.Inject(StructureId.ToString("N"), SortOrder, StringValue);
        }
    }
}