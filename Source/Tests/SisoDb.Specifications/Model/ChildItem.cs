using System.Collections.Generic;

namespace SisoDb.Specifications.Model
{
    public class ChildItem
    {
        public int Int { get; set; }

        public IList<int> IntegerList { get; set; }

        public IList<GrandChildItem> GrandChildItemList { get; set; }
    }
}