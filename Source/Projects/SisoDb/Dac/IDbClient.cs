using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Commands;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb.Dac
{
    public interface IDbClient : IDisposable
    {
        string DbName { get; }
        StorageProviders ProviderType { get; }
        IConnectionString ConnectionString { get; }
        IDbDataTypeTranslator DbDataTypeTranslator { get; }
        ISqlStatements SqlStatements { get; }

        void Flush();
        
        IDbCommand CreateCommand(CommandType commandType, string sql, params IDacParameter[] parameters);
        SqlBulkCopy GetBulkCopy(bool keepIdentities);

        void Drop(IStructureSchema structureSchema);
        void RebuildIndexes(IStructureSchema structureSchema);
        void DeleteById(ValueType structureId, IStructureSchema structureSchema);
        void DeleteByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema);
        void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, IStructureSchema structureSchema);
        void DeleteWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema);
        bool TableExists(string name);
        IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip);
        int RowCount(IStructureSchema structureSchema);
        int RowCountByQuery(IStructureSchema structureSchema, ISqlCommandInfo cmdInfo);
        long CheckOutAndGetNextIdentity(string entityHash, int numOfIds);
        IEnumerable<string> GetJson(IStructureSchema structureSchema);
        string GetJsonById(ValueType structureId, IStructureSchema structureSchema);
        IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema);
        IEnumerable<string> GetJsonWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema);

        void SingleResultSequentialReader(CommandType commandType, string sql, Action<IDataRecord> callback, params IDacParameter[] parameters);
    }
}