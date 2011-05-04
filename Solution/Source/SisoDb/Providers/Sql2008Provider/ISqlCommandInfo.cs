using System.Collections.Generic;
using SisoDb.Querying;

namespace SisoDb.Providers.Sql2008Provider
{
    public interface ISqlCommandInfo
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}