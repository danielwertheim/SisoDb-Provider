using SisoDb.Structures.Schemas;

namespace SisoDb.Structures.IdGenerators
{
    /// <summary>
    /// Uses native UuidCreateSequential in rpcrt4.dll, which does not
    /// work in WindowsXP and Azure. Use <see cref="CombGuidStructureIdGenerator"/> or
    /// <see cref="GuidStructureIdGenerator"/> instead.
    /// </summary>
    public class SequentialGuidStructureIdGenerator : IStructureIdGenerator
    {
        public virtual IStructureId Generate(IStructureSchema structureSchema)
        {
            return StructureId.Create(SequentialGuid.New());
        }

        public virtual IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];

            for (var c = 0; c < structureIds.Length; c++)
                structureIds[c] = StructureId.Create(SequentialGuid.New());

            return structureIds;
        }
    }
}