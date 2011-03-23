using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlDbClient : IDisposable
    {
        ISisoConnectionInfo ConnectionInfo { get; }
        IDbDataTypeTranslator DbDataTypeTranslator { get; set; }
        string DbName { get; }
        StorageProviders ProviderType { get; }
        ISqlStringsRepository SqlStringsRepository { get; }
        void Flush();
        SqlBulkCopy GetBulkCopy(bool keepIdentities);
        void CreateDatabase(string name);
        void CreateSysTables(string name);
        void DropDatabase(string name);
        bool DatabaseExists(string name);
        bool TableExists(string name);
        IList<SqlDbColumn> GetColumns(string tableName, params string[] namesToSkip);
        int RowCount(string tableName);
        int GetIdentity(string entityHash, int numOfIds);
        
        void DeleteById(ValueType structureId, string structureTableName, string indexesTableName, string uniquesTableName);
        void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName);
        void DeleteWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, string structureTableName, string indexesTableName, string uniquesTableName);
        
        string GetJsonById(ValueType structureId, string structureTableName);
        
        T ExecuteScalar<T>(CommandType commandType, string sql, params IQueryParameter[] parameters);

        void ExecuteSingleResultReader(CommandType commandType, string sql,
                                                       Action<IDataRecord> callback, params IQueryParameter[] parameters);

        int ExecuteNonQuery(CommandType commandType, string sql, params IQueryParameter[] parameters);
        IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters);
    }
}