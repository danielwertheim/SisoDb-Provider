using System;
using NCore;

namespace SisoDb.Specifications.Sql2008
{
    public class QueryGuidItem : QueryXItem<Guid>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject("\"" + StructureId.ToString("N") + "\"", SortOrder, StringValue);
        }
    }
}