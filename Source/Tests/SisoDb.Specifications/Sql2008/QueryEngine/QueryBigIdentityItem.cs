using System.Collections.Generic;
using NCore;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public class QueryBigIdentityItem
    {
        public const string JsonFormat = "{{\"StructureId\":\"{0}\",\"IntValue\":{1},\"StringValue\":\"{2}\"}}";

        public static IList<QueryGuidItem> CreateItems()
        {
            return new[]
            {
                new QueryGuidItem{IntValue = 1, StringValue = "A"},
                new QueryGuidItem{IntValue = 2, StringValue = "B"},
                new QueryGuidItem{IntValue = 3, StringValue = "C"},
                new QueryGuidItem{IntValue = 4, StringValue = "D"},
            };
        }

        public long StructureId { get; set; }

        public int IntValue { get; set; }

        public string StringValue { get; set; }

        public string AsJson()
        {
            return JsonFormat.Inject(StructureId, IntValue, StringValue);
        }
    }
}