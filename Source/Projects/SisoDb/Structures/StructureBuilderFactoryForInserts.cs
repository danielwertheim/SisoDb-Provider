using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
    public delegate IStructureBuilder StructureBuilderFactoryForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator);
}