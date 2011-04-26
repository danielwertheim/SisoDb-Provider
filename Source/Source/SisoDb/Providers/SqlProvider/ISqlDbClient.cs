using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlDbClient : IDisposable
    {
        string DbName { get; }

        ISisoConnectionInfo ConnectionInfo { get; }

        IDbDataTypeTranslator DbDataTypeTranslator { get; }

        ISqlStringsRepository SqlStringsRepository { get; }

        void Flush();

        IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters);

        SqlBulkCopy GetBulkCopy(bool keepIdentities);

        void DeleteById(ValueType sisoId, string structureTableName, string indexesTableName, string uniquesTableName);

        void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName);
        
        void DeleteWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, string structureTableName, string indexesTableName, string uniquesTableName);

        bool TableExists(string name);

        IList<SqlDbColumn> GetColumns(string tableName, params string[] namesToSkip);

        int RowCount(string tableName);

        int CheckOutAndGetNextIdentity(string entityHash, int numOfIds);

        string GetJsonById(ValueType sisoId, string structureTableName);

        IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, IdTypes idType, string structureTableName);

        void ExecuteSingleResultReader(CommandType commandType, string sql, Action<IDataRecord> callback, params IQueryParameter[] parameters);
    }
}