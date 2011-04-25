using System;
using System.Data;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlServerClient : IDisposable
    {
        ISisoConnectionInfo ConnectionInfo { get; }

        IDbDataTypeTranslator DbDataTypeTranslator { get; }

        ISqlStringsRepository SqlStringsRepository { get; }

        IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters);

        bool DatabaseExists(string name);

        void CreateDatabase(string name);

        void InitializeExistingDb(string name);

        void DropDatabase(string name);
    }
}