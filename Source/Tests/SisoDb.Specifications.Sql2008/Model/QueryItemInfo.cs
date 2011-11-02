namespace SisoDb.Specifications.Sql2008.Model
{
    public class QueryItemInfo
    {
        public int SortOrder { get; set; }

        public string StringValue { get; set; }

        public bool Matches<T>(QueryXItem<T> item) where T : struct
        {
            return SortOrder.Equals(item.SortOrder) 
                && StringValue.Equals(item.StringValue);
        }
    }
}