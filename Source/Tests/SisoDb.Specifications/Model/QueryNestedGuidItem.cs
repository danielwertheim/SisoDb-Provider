using System;
using System.Collections.Generic;
using SisoDb.NCore;

namespace SisoDb.Specifications.Model
{
    public class QueryNestedGuidItem
    {
        public const string JsonFormat = "{{\"StructureId\":{0},\"SortOrder\":{1},\"StringValue\":\"{2}\",\"Container\":{{\"NestedInt\":{3},\"NestedString\":\"{4}\"}}}}";

        public static IList<QueryNestedGuidItem> CreateFourNestedItems()
        {
            return new[]
            {
                new QueryNestedGuidItem{SortOrder = 1, StringValue = "A", Container = new Container{NestedInt = 11, NestedString = "Nested A"}},
                new QueryNestedGuidItem{SortOrder = 2, StringValue = "B", Container = new Container{NestedInt = 12, NestedString = "Nested B"}},
                new QueryNestedGuidItem{SortOrder = 3, StringValue = "C", Container = new Container{NestedInt = 13, NestedString = "Nested C"}},
                new QueryNestedGuidItem{SortOrder = 4, StringValue = "D", Container = new Container{NestedInt = 14, NestedString = "Nested D"}},
            };
        }
        public Guid StructureId { get; set; }

        public int SortOrder { get; set; }

        public string StringValue { get; set; }

        public Container Container { get; set; }

        public string AsJson()
        {
            return JsonFormat.Inject("\"" + StructureId.ToString("N") + "\"", SortOrder, StringValue, Container.NestedInt, Container.NestedString);
        }
    }
}