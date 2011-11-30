using NCore;

namespace SisoDb.Specifications.Model
{
    public class QueryStringItem : QueryXItem<string>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject("\"" + StructureId + "\"", SortOrder, IntegerValue, StringValue);
        }
    }
}