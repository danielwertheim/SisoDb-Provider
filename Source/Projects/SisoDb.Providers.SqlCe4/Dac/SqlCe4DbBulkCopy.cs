using System.Data;
using System.Data.SqlServerCe;
using ErikEJ.SqlCe;
using SisoDb.Dac;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4DbBulkCopy : IDbBulkCopy
    {
        private SqlCeBulkCopy _innerBulkCopy;

        public SqlCe4DbBulkCopy(SqlCeConnection connection, SqlCeBulkCopyOptions options, SqlCeTransaction transaction)
        {
            _innerBulkCopy = new SqlCeBulkCopy(connection, transaction, options)
            {
                NotifyAfter = 0
            };
        }

        public void Dispose()
        {
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
            set {  }
        }

        public void AddColumnMapping(string sourceFieldName, string destinationFieldName)
        {
            _innerBulkCopy.ColumnMappings.Add(new SqlCeBulkCopyColumnMapping(sourceFieldName, destinationFieldName));
        }

        public void Write(IDataReader reader)
        {
            _innerBulkCopy.WriteToServer(reader);
        }
    }
}