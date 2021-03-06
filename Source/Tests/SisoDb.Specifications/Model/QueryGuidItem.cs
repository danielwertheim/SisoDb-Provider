using System;
using SisoDb.NCore;

namespace SisoDb.Specifications.Model
{
	public class QueryGuidItem : QueryXItem<Guid>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject("\"" + StructureId.ToString("N") + "\"", SortOrder, IntegerValue, NullableIntegerValue, StringValue, GuidValue.ToString("N"), BoolValue.ToString().ToLower());
        }
    }
}