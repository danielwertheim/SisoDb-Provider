using System;
using System.Collections.Generic;

namespace SisoDb.Specifications.Sql2008.Model
{
    public abstract class QueryXItem<T> where T : struct
    {
        public const string JsonFormat = "{{\"StructureId\":{0},\"SortOrder\":{1},\"StringValue\":\"{2}\"}}";

        public static IList<TItem> CreateItems<TItem>(int numOfItems, Action<int, TItem> initializer) where TItem : new()
        {
            var items = new List<TItem>();

            for (var c = 0; c < numOfItems; c++)
            {
                var item = new TItem();
                
                initializer.Invoke(c, item);

                items.Add(item);
            }

            return items;
        }

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

        public static IList<TItem> CreateTenItems<TItem>() where TItem : QueryXItem<T>, new()
        {
            return new[]
            {
                new TItem{SortOrder = 1, StringValue = "A"},
                new TItem{SortOrder = 2, StringValue = "B"},
                new TItem{SortOrder = 3, StringValue = "C"},
                new TItem{SortOrder = 4, StringValue = "D"},
                new TItem{SortOrder = 5, StringValue = "E"},
                new TItem{SortOrder = 6, StringValue = "F"},
                new TItem{SortOrder = 7, StringValue = "G"},
                new TItem{SortOrder = 8, StringValue = "H"},
                new TItem{SortOrder = 9, StringValue = "I"},
                new TItem{SortOrder = 10, StringValue = "J"},
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