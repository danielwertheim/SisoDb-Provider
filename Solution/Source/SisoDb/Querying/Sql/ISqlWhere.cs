using System.Collections.Generic;

namespace SisoDb.Querying.Sql
{
    public interface ISqlWhere
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}