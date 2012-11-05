using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Structures.Schemas;

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

	    public virtual IStructureId Generate(IStructureSchema structureSchema)
    	{
    		var nextId = DbClient.CheckOutAndGetNextIdentity(structureSchema.Name, 1);

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return StructureId.Create((int)nextId);

            return StructureId.Create(nextId);
        }

        public virtual IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return GenerateIdentityStructureId(structureSchema, numOfIds);

            return GenerateBigIdentityStructureId(structureSchema, numOfIds);
        }

	    protected virtual IStructureId[] GenerateIdentityStructureId(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];
        	var startId = (int) DbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }

	    protected virtual IStructureId[] GenerateBigIdentityStructureId(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];
			var startId = DbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }
    }
}