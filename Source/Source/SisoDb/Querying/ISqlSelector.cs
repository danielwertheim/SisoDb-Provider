using System.Collections.Generic;

namespace SisoDb.Querying
{
    public interface ISqlSelector
    {
        string Sql { get; }
        IList<IQueryParameter> Parameters { get; }
    }
}