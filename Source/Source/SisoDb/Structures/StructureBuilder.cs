using SisoDb.Core;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureBuilder : IStructureBuilder
    {
        public IJsonSerializer JsonSerializer { get; private set; }

        public IStructureIdFactory IdFactory { get; private set; }

        public IStructureIndexesFactory IndexesFactory { get; private set; }

        public StructureBuilder(IJsonSerializer jsonSerializer, IStructureIdFactory structureIdFactory, IStructureIndexesFactory structureIndexesFactory)
        {
            JsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
            IdFactory = structureIdFactory.AssertNotNull("structureIdFactory");
            IndexesFactory = structureIndexesFactory.AssertNotNull("structureIndexesFactory");
        }

        public IStructure CreateStructure<T>(T item, IStructureSchema structureSchema)
            where T : class
        {
            var name = structureSchema.Name;
            var id = IdFactory.GetId(structureSchema, item);
            var indexes = IndexesFactory.GetIndexes(structureSchema, item, id);
            var json = JsonSerializer.ToJsonOrEmptyString(item);

            return new Structure(name, id, indexes, json);
        }

    }
}