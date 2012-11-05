using SisoDb.Structures.Schemas;

namespace SisoDb.Structures.IdGenerators
{
    public class SequentialGuidStructureIdGenerator : IStructureIdGenerator
    {
        public IStructureId Generate(IStructureSchema structureSchema)
        {
            return StructureId.Create(SequentialGuid.New());
        }

        public IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];

            for (var c = 0; c < structureIds.Length; c++)
                structureIds[c] = StructureId.Create(SequentialGuid.New());

            return structureIds;
        }
    }
}