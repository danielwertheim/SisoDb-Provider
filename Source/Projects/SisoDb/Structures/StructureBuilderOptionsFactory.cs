using NCore;
using PineCone.Structures;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb.Structures
{
    public static class StructureBuilderOptionsFactory
    {
        public static StructureBuilderOptions Create(StructureIdTypes structureIdType, IdentityStructureIdGenerator identityStructureIdGenerator)
        {
            var options = new StructureBuilderOptions { Serializer = new SerializerForStructureBuilder() };

            if (structureIdType.IsIdentity())
                options.IdGenerator = identityStructureIdGenerator;

            if (structureIdType == StructureIdTypes.Guid)
                options.IdGenerator = new GuidStructureIdGenerator();

            if(options.IdGenerator == null)
                throw new SisoDbException(ExceptionMessages.StructureBuilderOptionsFactory_Create.Inject(structureIdType));

            return options;
        }
    }
}