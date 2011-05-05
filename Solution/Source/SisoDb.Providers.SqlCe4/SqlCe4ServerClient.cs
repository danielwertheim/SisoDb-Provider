using System.Data;
using System.Data.SqlClient;
using SisoDb.Core;
using SisoDb.Providers.Dac;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlCe4
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider and
    /// executes command against the server not a specific database.
    /// </summary>
    public class SqlCe4ServerClient : ISqlServerClient
    {
        private SqlConnection _connection;

        public StorageProviders ProviderType { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

        public ISqlStringsRepository SqlStringsRepository { get; private set; }

        public SqlCe4ServerClient(SqlCe4ConnectionInfo connectionInfo)
        {
            connectionInfo.AssertNotNull("connectionInfo");

            ProviderType = connectionInfo.ProviderType;
            ConnectionString = connectionInfo.ConnectionString;

            SqlStringsRepository = new SqlStringsRepository(ProviderType);
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

        public bool DatabaseExists(string name)
        {
            var sql = SqlStringsRepository.GetSql("DatabaseExists");

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql, new QueryParameter("dbName", name)))
            {
                return cmd.GetScalarResult<int>() > 0;
            }
        }

        public void CreateDatabase(string name)
        {
            var sql = SqlStringsRepository.GetSql("CreateDatabase").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }

            InitializeExistingDb(name);
        }

        public void InitializeExistingDb(string name)
        {
            var firstSql = SqlStringsRepository.GetSql("Sys_Identities_CreateIfNotExists").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, firstSql))
            {
                cmd.ExecuteNonQuery();

                cmd.CommandText = SqlStringsRepository.GetSql("Sys_Types_CreateIfNotExists").Inject(name);
                cmd.ExecuteNonQuery();
            }
        }

        public void DropDatabase(string name)
        {
            var sql = SqlStringsRepository.GetSql("DropDatabase").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}