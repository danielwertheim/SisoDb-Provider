using System.Collections.Generic;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlCommandInfo
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}