using System.Collections.Generic;

namespace SisoDb.Querying
{
    public interface ISqlWhere
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}