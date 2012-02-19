using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4DbBulkCopy : IDbBulkCopy
    {
    	private readonly SqlCeConnection _connection;
        private readonly SqlCeTransaction _transaction;
        private readonly Dictionary<string, string> _columnMappings; 

        public SqlCe4DbBulkCopy(SqlCeConnection connection, SqlCeTransaction transaction)
        {
        	Ensure.That(connection, "connection").IsNotNull();
            Ensure.That(transaction, "transaction").IsNotNull();

        	_connection = connection;
            _transaction = transaction;
            _columnMappings = new Dictionary<string, string>();
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

			using(var cmd = _connection.CreateCommand())
			{
			    cmd.Transaction = _transaction;
				cmd.CommandText = DestinationTableName;
				cmd.CommandType = CommandType.TableDirect;
				using (var rsIn = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
				{
					var newRecord = rsIn.CreateRecord();

					while(reader.Read())
					{
						for (var i = 0; i < columnsCount; i++)
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