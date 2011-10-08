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
        void Drop(IStructureSchema structureSchema);
        void RebuildIndexes(IStructureSchema structureSchema);
        IDbCommand CreateCommand(CommandType commandType, string sql, params IDacParameter[] parameters);
        SqlBulkCopy GetBulkCopy(bool keepIdentities);
        void DeleteById(ValueType sisoId, IStructureSchema structureSchema);
        void DeleteByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema);
        void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, IStructureSchema structureSchema);
        void DeleteWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, IStructureSchema structureSchema);
        bool TableExists(string name);
        IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip);
        int RowCount(IStructureSchema structureSchema);
        int RowCountByQuery(IStructureSchema structureSchema, ISqlCommandInfo cmdInfo);
        long CheckOutAndGetNextIdentity(string entityHash, int numOfIds);
        string GetJsonById(ValueType sisoId, IStructureSchema structureSchema);
        IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema);
        IEnumerable<string> GetJsonWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, IStructureSchema structureSchema);

        void SingleResultSequentialReader(CommandType commandType, string sql, Action<IDataRecord> callback, params IDacParameter[] parameters);
    }
}