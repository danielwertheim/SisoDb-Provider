using System.Collections.Generic;

namespace SisoDb.Querying
{
    internal interface ISqlQuery
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}