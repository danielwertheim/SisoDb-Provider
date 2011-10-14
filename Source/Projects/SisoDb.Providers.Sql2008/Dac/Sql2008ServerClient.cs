using System.Data;
using System.Data.SqlClient;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Sql2008.DbSchema;

namespace SisoDb.Sql2008.Dac
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider and
    /// executes command against the server not a specific database.
    /// </summary>
    public class Sql2008ServerClient : IServerClient
    {
        private SqlConnection _connection;

        public StorageProviders ProviderType { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

        public ISqlStatements SqlStatements { get; private set; }

        public Sql2008ServerClient(ISisoConnectionInfo connectionInfo)
        {
            Ensure.That(() => connectionInfo).IsNotNull();

            ProviderType = connectionInfo.ProviderType;
            ConnectionString = connectionInfo.ServerConnectionString;

            SqlStatements = Sql2008Statements.Instance;//TODO: ProviderFactory
            DbDataTypeTranslator = new Sql2008DataTypeTranslator();//TODO: ProviderFactory

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
            var sql = SqlStatements.GetSql("DatabaseExists");

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql, new DacParameter("dbName", name)))
            {
                return cmd.GetScalarResult<int>() > 0;
            }
        }

        public void CreateDatabase(string name)
        {
            var sql = SqlStatements.GetSql("CreateDatabase").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }

            InitializeExistingDb(name);
        }

        public void InitializeExistingDb(string name)
        {
            var sqlCreateIdentitiesTables = SqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sqlCreateIdentitiesTables))
            {
                cmd.ExecuteNonQuery();

                cmd.CommandText = SqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(name);
                cmd.ExecuteNonQuery();
            }
        }

        public void DropDatabaseIfExists(string name)
        {
            var sql = SqlStatements.GetSql("DropDatabase").Inject(name);

            using (var cmd = _connection.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}