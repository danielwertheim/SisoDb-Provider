using System;
using System.Data;
using System.Data.SqlServerCe;
using SisoDb.Core;
using SisoDb.Providers;

namespace SisoDb.SqlCe4.Dac
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider and
    /// executes command against the server not a specific database.
    /// </summary>
    internal class SqlCe4ServerClient : IDisposable
    {
        private readonly SqlCe4ConnectionInfo _connectionInfo;

        //private SqlCeEngine _sqlCeEngine;

        private SqlCeConnection _connection;

        //internal StorageProviders ProviderType { get; private set; }

        //internal IConnectionString ConnectionString { get; private set; }

        //internal IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

        internal ISqlStatements SqlStatements { get; private set; }

        internal SqlCe4ServerClient(SqlCe4ConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo.AssertNotNull("connectionInfo");

            SqlStatements = SqlCe4Statements.Instance;
            //DbDataTypeTranslator = new TSqlDbDataTypeTranslator();

            _connection = new SqlCeConnection(_connectionInfo.ConnectionString.PlainString);
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

        internal void InitializeExistingDb()
        {
            var sqlCreateIdentitiesTables = SqlStatements.GetSql("Sys_Identities_Create");
            
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCreateIdentitiesTables;

                cmd.ExecuteNonQuery();

                //cmd.CommandText = SqlStatements.GetSql("Sys_Types_Create");
                //cmd.ExecuteNonQuery();
            }
        }
    }
}