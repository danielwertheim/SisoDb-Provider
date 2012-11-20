using SisoDb.Dac;

namespace SisoDb.Querying.Sql
{
    public interface IDbQuery
    {
        string Sql { get; }
        IDacParameter[] Parameters { get; }
        bool IsCacheable { get; }
        bool IsEmpty { get; }
    }
}