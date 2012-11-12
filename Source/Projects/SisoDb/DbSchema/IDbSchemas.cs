using SisoDb.Dac;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemas
    {
    	void ClearCache();
        void RemoveFromCache(string structureSchemaName);
    	void RemoveFromCache(IStructureSchema structureSchema);

        void Drop(IStructureSchema structureSchema, IDbClient dbClient);
        void Upsert(IStructureSchema structureSchema, IDbClient dbClient);
    }
}