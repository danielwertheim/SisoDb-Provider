using System.Data;

namespace SisoDb.Dac
{
    public interface IAdoDriver
    {
        IDbConnection CreateConnection(string connectionString);
        IDbCommand CreateCommand(IDbConnection connection,  string sql, IDbTransaction transaction = null, params IDacParameter[] parameters);
        IDbCommand CreateSpCommand(IDbConnection connection, string spName, IDbTransaction transaction = null, params IDacParameter[] parameters);
        void AddCommandParametersTo(IDbCommand cmd, params IDacParameter[] parameters);
    }
}