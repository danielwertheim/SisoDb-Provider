using System;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlStrings;

namespace SisoDb.Providers.Sql2008
{
    public interface ISqlServerClient : IDisposable
    {
        ISisoConnectionInfo ConnectionInfo { get; }

        IDbDataTypeTranslator DbDataTypeTranslator { get; }

        ISqlStringsRepository SqlStringsRepository { get; }
        
        bool DatabaseExists(string name);

        void CreateDatabase(string name);

        void InitializeExistingDb(string name);

        void DropDatabase(string name);
    }
}