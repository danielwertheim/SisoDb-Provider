using System;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests
{
    public class FakeSerializer : IStructureSerializer
    {
        public Func<object, IStructureSchema, string> OnSerialize;

        public string Serialize<T>(T structure, IStructureSchema structureSchema) where T : class
        {
            return OnSerialize(structure, structureSchema);
        }
    }
}