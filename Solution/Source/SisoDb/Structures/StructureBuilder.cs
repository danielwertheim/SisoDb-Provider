using SisoDb.Core;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureBuilder : IStructureBuilder
    {
        public IJsonSerializer JsonSerializer { get; private set; }

        public ISisoIdFactory IdFactory { get; private set; }

        public IStructureIndexesFactory IndexesFactory { get; private set; }

        public StructureBuilder(IJsonSerializer jsonSerializer, ISisoIdFactory sisoIdFactory, IStructureIndexesFactory structureIndexesFactory)
        {
            JsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
            IdFactory = sisoIdFactory.AssertNotNull("sisoIdFactory");
            IndexesFactory = structureIndexesFactory.AssertNotNull("structureIndexesFactory");
        }

        public IStructure CreateStructure<T>(T item, IStructureSchema structureSchema)
            where T : class
        {
            var id = IdFactory.GetId(structureSchema, item);

            return new Structure(
                structureSchema.Name, 
                id,
                IndexesFactory.GetIndexes(structureSchema, item, id),
                JsonSerializer.ToJsonOrEmptyString(item));
        }
    }
}