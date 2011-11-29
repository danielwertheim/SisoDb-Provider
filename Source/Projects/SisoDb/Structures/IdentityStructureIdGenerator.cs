using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.Structures
{
    public class IdentityStructureIdGenerator : IStructureIdGenerator
    {
        private readonly GetNextIdentity _getNextIdentity;

        public IdentityStructureIdGenerator(GetNextIdentity getNextIdentity)
        {
            _getNextIdentity = getNextIdentity;
        }

        public IStructureId Generate(IStructureSchema structureSchema)
        {
            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return StructureId.Create((int)_getNextIdentity.Invoke(structureSchema, 1));

            return StructureId.Create(_getNextIdentity.Invoke(structureSchema, 1));
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
            var startId = (int)_getNextIdentity.Invoke(structureSchema, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }

        private IStructureId[] GenerateBigIdentityStructureId(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];
            var startId = _getNextIdentity.Invoke(structureSchema, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                structureIds[c] = StructureId.Create(startId++);

            return structureIds;
        }
    }
}