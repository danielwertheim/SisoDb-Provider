using System.Collections.Generic;
using SisoDb.Dac;

namespace SisoDb.Querying.Sql
{
    public interface IDbQuery 
    {
        string Sql { get; }
        IDacParameter[] Parameters { get; }
        bool IsEmpty { get; }
    }
}