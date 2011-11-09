namespace SisoDb.Specifications.Model
{
    public class QueryNestedItemInfo
    {
        public int SortOrder { get; set; }

        public string StringValue { get; set; }
        
        public Container Container { get; set; }

        public bool Matches(QueryNestedGuidItem item)
        {
            return
                SortOrder.Equals(item.SortOrder) 
                && StringValue.Equals(item.StringValue) 
                && Container.NestedInt.Equals(item.Container.NestedInt)
                && Container.NestedString.Equals(item.Container.NestedString);
        }
    }
}