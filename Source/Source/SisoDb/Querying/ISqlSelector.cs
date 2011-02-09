using System.Collections.Generic;

namespace SisoDb.Querying
{
    internal interface ISqlSelector
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}