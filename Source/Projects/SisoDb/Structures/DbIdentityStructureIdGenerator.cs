using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
	public class DbIdentityStructureIdGenerator : IIdentityStructureIdGenerator
	{
        private readonly CheckOutAngGetNextIdentity _checkOutAndGetNextIdentity;

        public DbIdentityStructureIdGenerator(CheckOutAngGetNextIdentity checkOutAndGetNextIdentity)
        {
            Ensure.That(checkOutAndGetNextIdentity, "checkOutAndGetNextIdentity").IsNotNull();
            
            _checkOutAndGetNextIdentity = checkOutAndGetNextIdentity;
        }

	    public IStructureId Generate(IStructureSchema structureSchema)
    	{
    		var nextId = _checkOutAndGetNextIdentity(structureSchema, 1);

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
        	var startId = (int) _checkOutAndGetNextIdentity(structureSchema, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }

        private IStructureId[] GenerateBigIdentityStructureId(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];
			var startId = _checkOutAndGetNextIdentity(structureSchema, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }
    }
}