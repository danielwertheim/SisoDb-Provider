using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaSynchronizer
    {
		void Synchronize(IStructureSchema structureSchema, IDbClient dbClient);
    }
}