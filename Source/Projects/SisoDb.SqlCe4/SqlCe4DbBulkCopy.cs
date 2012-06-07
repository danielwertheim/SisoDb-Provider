using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using SisoDb.Dac;
using SisoDb.SqlCe4.Profiling;

namespace SisoDb.SqlCe4
{
    public class SqlCe4DbBulkCopy : IDbBulkCopy
    {
    	private SqlCeConnection _connection;
        private SqlCeTransaction _transaction;
        private Dictionary<string, string> _columnMappings; 

        public SqlCe4DbBulkCopy(IDbClient dbClient)
        {
            Ensure.That(dbClient, "dbClient").IsNotNull();

            Initialize(dbClient.Connection);
        }

        public SqlCe4DbBulkCopy(ITransactionalDbClient dbClient)
        {
            Ensure.That(dbClient, "dbClient").IsNotNull();

            Initialize(dbClient.Connection, dbClient.Transaction);
        }

        private void Initialize(IDbConnection connection, IDbTransaction transaction = null)
        {
            _columnMappings = new Dictionary<string, string>();
            _connection = connection.ToSqlCeConnection();
            _transaction = transaction.ToSqlCeTransaction();
        }

    	public void Dispose()
        {
			GC.SuppressFinalize(this);
        }

		public string DestinationTableName { private get; set; }

		public int BatchSize { private get; set; }

        public void AddColumnMapping(string sourceFieldName, string destinationFieldName)
        {
			_columnMappings.Add(sourceFieldName, destinationFieldName);
        }

        public void Write(IDataReader reader)
        {
        	var columnsCount = _columnMappings.Count;
            const int rowIdIndex = 0;
            const int offsetCausedByRowId = rowIdIndex + 1;

			using(var cmd = _connection.CreateCommand())
			{
                if(_transaction != null)
			        cmd.Transaction = _transaction;
				
                cmd.CommandText = DestinationTableName;
				cmd.CommandType = CommandType.TableDirect;
				using (var rsIn = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
				{
					var newRecord = rsIn.CreateRecord();
					while(reader.Read())
					{
                        for (var i = offsetCausedByRowId; i <= columnsCount; i++)
						{
							newRecord.SetValue(i, reader.GetValue(i));
						}
						rsIn.Insert(newRecord);
					}
				}
			}
        }
    }
}