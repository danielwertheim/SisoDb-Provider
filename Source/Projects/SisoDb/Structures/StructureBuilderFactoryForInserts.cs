using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
    public delegate IStructureBuilder StructureBuilderFactoryForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator);
}