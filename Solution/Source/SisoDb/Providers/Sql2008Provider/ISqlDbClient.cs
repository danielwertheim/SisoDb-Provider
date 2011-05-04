using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008Provider.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb.Providers.Sql2008Provider
{
    public interface ISqlDbClient : IDisposable
    {
        string DbName { get; }

        ISisoConnectionInfo ConnectionInfo { get; }

        IDbDataTypeTranslator DbDataTypeTranslator { get; }

        ISqlStringsRepository SqlStringsRepository { get; }

        void Flush();

        void RebuildIndexes(string structureTableName, string indexesTableName, string uniquesTableName);

        IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters);

        SqlBulkCopy GetBulkCopy(bool keepIdentities);

        void DeleteById(ValueType sisoId, string structureTableName, string indexesTableName, string uniquesTableName);

        void DeleteByIds(IEnumerable<ValueType> ids, IdTypes idType, string structureTableName, string indexesTableName, string uniquesTableName);

        void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName);
        
        void DeleteWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, string structureTableName, string indexesTableName, string uniquesTableName);

        bool TableExists(string name);

        IList<SqlDbColumn> GetColumns(string tableName, params string[] namesToSkip);

        int RowCount(string structureTableName);

        int RowCountByQuery(string tableName, ISqlCommandInfo cmdInfo);

        int CheckOutAndGetNextIdentity(string entityHash, int numOfIds);

        string GetJsonById(ValueType sisoId, string structureTableName);

        IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, IdTypes idType, string structureTableName);

        IEnumerable<string> GetJsonWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, string structureTableName);

        void SingleResultSequentialReader(CommandType commandType, string sql, Action<IDataRecord> callback, params IQueryParameter[] parameters);
    }
}