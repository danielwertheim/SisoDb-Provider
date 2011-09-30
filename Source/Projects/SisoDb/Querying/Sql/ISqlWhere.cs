using System.Collections.Generic;
using SisoDb.Dac;

namespace SisoDb.Querying.Sql
{
    public interface ISqlWhere
    {
        string Sql { get; }
        IList<IDacParameter> Parameters { get; }
    }
}