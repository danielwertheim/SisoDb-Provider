using System.Collections.Generic;

namespace SisoDb.Specifications.Sql2008.Model
{
    public class ChildItem
    {
        public int Int { get; set; }

        public IList<int> IntegerList { get; set; }

        public IList<GrandChildItem> GrandChildItemList { get; set; }
    }
}