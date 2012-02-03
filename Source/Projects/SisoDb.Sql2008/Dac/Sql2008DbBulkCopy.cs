using System;
using System.Data;
using System.Data.SqlClient;
using SisoDb.Dac;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008DbBulkCopy : IDbBulkCopy
    {
        private SqlBulkCopy _innerBulkCopy;

        public Sql2008DbBulkCopy(SqlConnection connection, SqlTransaction transaction)
        {
			_innerBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
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