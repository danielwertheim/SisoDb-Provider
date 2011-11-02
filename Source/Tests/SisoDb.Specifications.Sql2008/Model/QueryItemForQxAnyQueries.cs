using System;
using System.Collections.Generic;

namespace SisoDb.Specifications.Sql2008.Model
{
    public class QueryItemForQxAnyQueries
    {
        public Guid StructureId { get; set; }

        public string[] Strings { get; set; }

        public int[] Integers { get; set; }

        public IList<string> StringList { get; set; }

        public IList<decimal> DecimalList { get; set; }

        public IList<ChildItem> ChildItemList { get; set; }
    }
}