using System;
using NCore;

namespace SisoDb.Specifications.Model
{
	public class QueryGuidItem : QueryXItem<Guid>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject("\"" + StructureId.ToString("N") + "\"", SortOrder, IntegerValue, NullableIntegerValue, StringValue);
        }
    }
}