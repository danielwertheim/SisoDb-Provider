using System;
using System.Data;
using System.Data.SqlClient;
using SisoDb.Core;
using SisoDb.Providers.Dac;
using SisoDb.Providers.DbSchema;
using SisoDb.Querying;

namespace SisoDb.Providers.Sql2008.Dac
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider and
    /// executes command against the server not a specific database.
    /// </summary>
    public class SqlServerClient : IDisposable
    {
        private SqlConnection _connection;

        internal StorageProviders ProviderType { get; private set; }

        internal IConnectionString ConnectionString { get; private set; }
        
        internal IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

        internal ISqlStatements SqlStatements { get; private set; }

        internal SqlServerClient(SqlConnectionInfo connectionInfo)
        {
            connectionInfo.AssertNotNull("connectionInfo");

            ProviderType = connectionInfo.ProviderType;
            ConnectionString = connectionInfo.ServerConnectionString;
            
            SqlStatements = Sql2008Statements.Instance;
            DbDataTypeTranslator = new SqlDbDataTypeTranslator();

            _connection = new SqlConnection(ConnectionString.PlainString);
            _connection.Open();
        }

        public void Dispose()
        {
            if (_connection == null)
                return;

            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
            _connection = null;
        }

        internal bool DatabaseExists(string name)
        {
            var sql = SqlStatements.GetSql("DatabaseExists");

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql, new QueryParameter("dbName", name)))
            {
                return cmd.GetScalarResult<int>() > 0;
            }
        }

        internal void CreateDatabase(string name)
        {
            var sql = SqlStatements.GetSql("CreateDatabase").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }

            InitializeExistingDb(name);
        }

        internal void InitializeExistingDb(string name)
        {
            var sqlCreateIdentitiesTables = SqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sqlCreateIdentitiesTables))
            {
                cmd.ExecuteNonQuery();

                cmd.CommandText = SqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(name);
                cmd.ExecuteNonQuery();
            }
        }

        internal void DropDatabaseIfExists(string name)
        {
            var sql = SqlStatements.GetSql("DropDatabase").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}