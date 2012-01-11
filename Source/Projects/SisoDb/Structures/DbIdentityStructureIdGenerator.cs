using EnsureThat;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.Structures
{
	public class DbIdentityStructureIdGenerator : IIdentityStructureIdGenerator
    {
    	protected readonly IDbClient DbClient;

        public DbIdentityStructureIdGenerator(IDbClient dbClient)
        {
        	Ensure.That(dbClient, "dbClient").IsNotNull();
        	DbClient = dbClient;
        }

    	public IStructureId Generate(IStructureSchema structureSchema)
    	{
    		var nextId = DbClient.CheckOutAndGetNextIdentity(structureSchema.Name, 1);

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return StructureId.Create((int)nextId);

            return StructureId.Create(nextId);
        }

        public IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return GenerateIdentityStructureId(structureSchema, numOfIds);

            return GenerateBigIdentityStructureId(structureSchema, numOfIds);
        }

        private IStructureId[] GenerateIdentityStructureId(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];
        	var startId = (int) DbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }

        private IStructureId[] GenerateBigIdentityStructureId(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];
			var startId = DbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }
    }
}