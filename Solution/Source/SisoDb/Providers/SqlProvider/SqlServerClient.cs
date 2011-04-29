using System.Data;
using System.Data.SqlClient;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlProvider
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider and
    /// executes command against the server not a specific database.
    /// </summary>
    public class SqlServerClient : ISqlServerClient
    {
        private SqlConnection _connection;

        public StorageProviders ProviderType
        {
            get { return ConnectionInfo.ProviderType; }
        }

        public ISisoConnectionInfo ConnectionInfo { get; private set; }

        public IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

        public ISqlStringsRepository SqlStringsRepository { get; private set; }

        public SqlServerClient(ISisoConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");
            SqlStringsRepository = new SqlStringsRepository(ProviderType);
            DbDataTypeTranslator = new SqlDbDataTypeTranslator();

            _connection = new SqlConnection(ConnectionInfo.ConnectionString.PlainString);
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