using System.Collections.Generic;

namespace SisoDb.Querying
{
    public interface ISqlCommandInfo
    {
        string Value { get; }
        IEnumerable<IQueryParameter> Parameters { get; }
    }
}