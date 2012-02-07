using System;
using System.Data;
using System.Data.SqlClient;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.Sql2012.Dac
{
    public class Sql2012DbBulkCopy : IDbBulkCopy
    {
        private SqlBulkCopy _innerBulkCopy;

        public Sql2012DbBulkCopy(SqlConnection connection)
        {
            Ensure.That(connection, "connection").IsNotNull();

			_innerBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null)
            {
                NotifyAfter = 0
            };
        }

        public void Dispose()
        {
			GC.SuppressFinalize(this);
			if(_innerBulkCopy != null)
            {
                _innerBulkCopy.Close();
                _innerBulkCopy = null;
            }
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
            _innerBulkCopy.WriteToServer(reader);
        }
    }
}