using System.Collections.Generic;

namespace SisoDb.Specifications.Sql2008
{
    public abstract class QueryXItem<T> where T : struct
    {
        public const string JsonFormat = "{{\"StructureId\":{0},\"SortOrder\":{1},\"StringValue\":\"{2}\"}}";

        public static IList<TItem> CreateFourItems<TItem>() where TItem : QueryXItem<T>, new()
        {
            return new[]
            {
                new TItem{SortOrder = 1, StringValue = "A"},
                new TItem{SortOrder = 2, StringValue = "B"},
                new TItem{SortOrder = 3, StringValue = "C"},
                new TItem{SortOrder = 4, StringValue = "D"},
            };
        }

        public static IList<TItem> CreateFourUnorderedItems<TItem>() where TItem : QueryXItem<T>, new()
        {
            return new[]
            {
                new TItem{SortOrder = 2, StringValue = "D"},
                new TItem{SortOrder = 2, StringValue = "C"},
                new TItem{SortOrder = 1, StringValue = "B"},
                new TItem{SortOrder = 1, StringValue = "A"},
            };
        }

        public T StructureId { get; set; }

        public int SortOrder { get; set; }

        public string StringValue { get; set; }

        public abstract string AsJson();
    }
}