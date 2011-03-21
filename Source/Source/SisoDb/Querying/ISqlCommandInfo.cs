using System.Collections.Generic;

namespace SisoDb.Querying
{
    public interface ISqlCommandInfo
    {
        string Value { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}