using System;
using NCore;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    public class QueryGuidItem : QueryXItem<Guid>
    {
        public override string AsJson()
        {
            return JsonFormat.Inject("\"" + StructureId.ToString("N") + "\"", SortOrder, StringValue);
        }
    }
}