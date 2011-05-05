using System.Collections.Generic;
using SisoDb.Querying;

namespace SisoDb.Commands
{
    public interface ISqlCommandInfo
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}