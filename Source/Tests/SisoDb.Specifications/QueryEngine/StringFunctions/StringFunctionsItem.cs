using System;
using System.Collections.Generic;

namespace SisoDb.Specifications.QueryEngine.StringFunctions
{
    public class StringFunctionsItem
    {
        public Guid StructureId { get; set; }

        public string String1 { get; set; }

        public static IEnumerable<StringFunctionsItem> CreateItems(int numOfItems, string prefix)
        {
            for (var c = 0; c < numOfItems; c++)
            {
                yield return new StringFunctionsItem { String1 = prefix + c };
            }
        }
    }
}