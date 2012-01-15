using System;
using System.Collections.Generic;
using System.Data;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;

namespace SisoDb.Dac
{
    public interface IDbClient : IDisposable
    {
        bool IsTransactional { get; }
        void Flush();

		IDbBulkCopy GetBulkCopy();
        void ExecuteNonQuery(string sql, params IDacParameter[] parameters);
		void SingleResultSequentialReader(string sql, Action<IDataRecord> callback, params IDacParameter[] parameters);
        long CheckOutAndGetNextIdentity(string entityName, int numOfIds);
        void Drop(IStructureSchema structureSchema);
		bool TableExists(string name);
		IndexesTableStatuses GetIndexesTableStatuses(IndexesTableNames names);

		void DeleteById(IStructureId structureId, IStructureSchema structureSchema);
        void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);
        void DeleteByQuery(DbQuery query, IStructureSchema structureSchema);
        void DeleteWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema);
        
        int RowCount(IStructureSchema structureSchema);
        int RowCountByQuery(IStructureSchema structureSchema, DbQuery query);
        
		string GetJsonById(IStructureId structureId, IStructureSchema structureSchema);
    	IEnumerable<string> GetJsonOrderedByStructureId(IStructureSchema structureSchema);
		IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);
        IEnumerable<string> GetJsonWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema);
		
		IEnumerable<string> YieldJson(string sql, params IDacParameter[] parameters);
    	IEnumerable<string> YieldJsonBySp(string sql, params IDacParameter[] parameters);
    }
}