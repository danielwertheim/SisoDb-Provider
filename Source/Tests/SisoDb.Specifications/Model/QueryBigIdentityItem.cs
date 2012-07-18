using SisoDb.NCore;

namespace SisoDb.Specifications.Model
{
    public class QueryBigIdentityItem : QueryXItem<long>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject(StructureId, SortOrder, IntegerValue, NullableIntegerValue, StringValue);
        }
    }
}