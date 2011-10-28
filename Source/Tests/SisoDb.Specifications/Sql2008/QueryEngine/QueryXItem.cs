namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public abstract class QueryXItem<T> where T : struct
    {
        public const string JsonFormat = "{{\"StructureId\":\"{0}\",\"SortOrder\":{1},\"StringValue\":\"{2}\"}}";

        public T StructureId { get; set; }

        public int SortOrder { get; set; }

        public string StringValue { get; set; }

        public abstract string AsJson();
    }
}