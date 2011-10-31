using NCore;

namespace SisoDb.Specifications.Sql2008
{
    public class QueryBigIdentityItem : QueryXItem<long>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject(StructureId, SortOrder, StringValue);
        }
    }
}