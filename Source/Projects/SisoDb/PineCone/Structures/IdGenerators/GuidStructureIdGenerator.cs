using System;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone.Structures.IdGenerators
{
    public class GuidStructureIdGenerator : IStructureIdGenerator 
    {
        public IStructureId Generate(IStructureSchema structureSchema)
        {
			return StructureId.Create(Guid.NewGuid());
        }

        public IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
			var structureIds = new IStructureId[numOfIds];

			for (var c = 0; c < structureIds.Length; c++)
				structureIds[c] = StructureId.Create(Guid.NewGuid());

			return structureIds;
        }
    }
}