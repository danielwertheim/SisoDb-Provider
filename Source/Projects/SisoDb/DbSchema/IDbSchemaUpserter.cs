using SisoDb.Dac;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema, IDbClient dbClient, bool allowDynamicSchemaCreation, bool synchronizeSchemaChanges);
    }
}