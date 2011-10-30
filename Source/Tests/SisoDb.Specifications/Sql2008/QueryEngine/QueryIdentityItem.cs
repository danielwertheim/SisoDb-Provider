using NCore;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public class QueryIdentityItem : QueryXItem<int>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject(StructureId, SortOrder, StringValue);
        }
    }
}