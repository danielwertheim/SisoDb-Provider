using System;
using System.Collections.Generic;
using System.Data;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Dac
{
    /// <summary>
    /// Defines operations that Siso needs to perform on a db-level. For server-level
    /// operations, see <see cref="IServerClient"/>.
    /// </summary>
    public interface IDbClient : IDisposable
    {
        Guid Id { get; }
        bool IsTransactional { get; }
        bool IsAborted { get; }
        bool IsFailed { get; }
        IAdoDriver Driver { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        Action<IDbClient> OnCompleted { set; }
        Action<IDbClient> AfterCommit { set; }
        Action<IDbClient> AfterRollback { set; }

        void Abort();
        void MarkAsFailed();
        IDbBulkCopy GetBulkCopy();

        void ExecuteNonQuery(string sql, params IDacParameter[] parameters);
        void ExecuteNonQuery(string[] sqls, params IDacParameter[] parameters);
        T ExecuteScalar<T>(string sql, params IDacParameter[] parameters);
        void SingleResultSequentialReader(string sql, Action<IDataRecord> callback, params IDacParameter[] parameters);
        
        long CheckOutAndGetNextIdentity(string entityName, int numOfIds);
        void RenameStructureSet(string oldStructureName, string newStructureName);
        void Drop(IStructureSchema structureSchema);
        void UpsertSp(string name, string createSpSql);
        void Reset();
        void ClearQueryIndexes(IStructureSchema structureSchema);
        
        bool TableExists(string name);
        ModelTablesInfo GetModelTablesInfo(IStructureSchema structureSchema);
        ModelTableStatuses GetModelTableStatuses(ModelTableNames names);

        void DeleteAll(IStructureSchema structureSchema);
        void DeleteAllExceptIds(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema);
		void DeleteById(IStructureId structureId, IStructureSchema structureSchema);
        void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);
        void DeleteByQuery(IDbQuery query, IStructureSchema structureSchema);
        void DeleteIndexesAndUniquesById(IStructureId structureId, IStructureSchema structureSchema);

        int RowCount(IStructureSchema structureSchema);
        int RowCountByQuery(IStructureSchema structureSchema, IDbQuery query);

        bool Any(IStructureSchema structureSchema);
        bool Any(IStructureSchema structureSchema, IDbQuery query);
        bool Exists(IStructureSchema structureSchema, IStructureId structureId);

		string GetJsonById(IStructureId structureId, IStructureSchema structureSchema);
        string GetJsonByIdWithLock(IStructureId structureId, IStructureSchema structureSchema);

    	IEnumerable<string> GetJsonOrderedByStructureId(IStructureSchema structureSchema);
		IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);
		
		IEnumerable<string> YieldJson(string sql, params IDacParameter[] parameters);
    	IEnumerable<string> YieldJsonBySp(string sql, params IDacParameter[] parameters);

        void SingleInsertStructure(IStructure structure, IStructureSchema structureSchema);
        void SingleInsertOfValueTypeIndex(IStructureIndex structureIndex, string valueTypeIndexesTableName);
        void SingleInsertOfStringTypeIndex(IStructureIndex structureIndex, string stringishIndexesTableName);
        void SingleInsertOfUniqueIndex(IStructureIndex uniqueStructureIndex, IStructureSchema structureSchema);
        void SingleUpdateOfStructure(IStructure structure, IStructureSchema structureSchema);
    }
}