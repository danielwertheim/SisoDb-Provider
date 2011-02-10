using System.Collections.Generic;

namespace SisoDb.Querying
{
    internal interface ISqlCommandInfo
    {
        string Value { get; }
        IEnumerable<IQueryParameter> Parameters { get; }
    }
}