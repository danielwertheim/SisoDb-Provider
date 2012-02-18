using System.Data;

namespace SisoDb
{
    public interface ISisoDbDatabaseTransaction : ISisoDbTransaction
    {
        IDbTransaction InnerTransaction { get; }
    }
}