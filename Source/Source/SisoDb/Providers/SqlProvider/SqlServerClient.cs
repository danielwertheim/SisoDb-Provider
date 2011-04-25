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

        public IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandType = commandType;

            if (!string.IsNullOrWhiteSpace(sql))
                cmd.CommandText = sql;
            
            foreach (var queryParameter in parameters)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = queryParameter.Name;
                parameter.Value = queryParameter.Value;

                cmd.Parameters.Add(parameter);
            }

            return cmd;
        }

        public bool DatabaseExists(string name)
        {
            var sql = SqlStringsRepository.GetSql("DatabaseExists");

            using (var cmd = CreateCommand(CommandType.Text, sql, new QueryParameter("dbName", name)))
            {
                return cmd.GetScalarResult<int>() > 0;
            }
        }

        public void CreateDatabase(string name)
        {
            var sql = SqlStringsRepository.GetSql("CreateDatabase").Inject(name);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();

                ExecuteCreateSysTables(name, cmd);
            }
        }

        private void ExecuteCreateSysTables(string name, IDbCommand cmd)
        {
            cmd.CommandText = SqlStringsRepository.GetSql("Sys_Identities_CreateIfNotExists").Inject(name);
            cmd.ExecuteNonQuery();
        }

        public void InitializeExistingDb(string name)
        {
            using (var cmd = CreateCommand(CommandType.Text, string.Empty))
            {
                ExecuteCreateSysTables(name, cmd);
            }
        }

        public void DropDatabase(string name)
        {
            var sql = SqlStringsRepository.GetSql("DropDatabase").Inject(name);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}