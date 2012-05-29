using System;
using System.Data;
using System.Data.SqlClient;
using EnsureThat;
using SisoDb.Dac;
using SisoDb.Dac.Profiling;

namespace SisoDb.SqlServer
{
    public class SqlServerBulkCopy : IDbBulkCopy
    {
        private IDbClient _dbClient;
        private SqlBulkCopy _innerBulkCopy;

        public SqlServerBulkCopy(ITransactionalDbClient dbClient)
        {
            Ensure.That(dbClient, "dbClient").IsNotNull();

            _dbClient = dbClient;
            Initialize(dbClient.Connection, dbClient.Transaction);
        }

        private void Initialize(IDbConnection connection, IDbTransaction transaction = null)
        {
            _innerBulkCopy = new SqlBulkCopy(
                connection.ToSqlConnection(),
                SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls,
                transaction.ToSqlTransaction())
            {
                NotifyAfter = 0
            };
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_innerBulkCopy != null)
            {
                _innerBulkCopy.Close();
                _innerBulkCopy = null;
            }

            _dbClient = null;
        }

        public string DestinationTableName
        {
            set { _innerBulkCopy.DestinationTableName = value; }
        }

        public int BatchSize
        {
            set { _innerBulkCopy.BatchSize = value; }
        }

        public void AddColumnMapping(string sourceFieldName, string destinationFieldName)
        {
            _innerBulkCopy.ColumnMappings.Add(sourceFieldName, destinationFieldName);
        }

        public void Write(IDataReader reader)
        {
            _dbClient.ExecuteNonQuery("SET XACT_ABORT ON;");
            _innerBulkCopy.WriteToServer(reader);
        }
    }
}