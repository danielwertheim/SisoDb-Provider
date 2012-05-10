using System;
using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        Func<IStructureSchema, IStructureIdGenerator> StructureIdGeneratorFn { get; set; }

        IStructureBuilder ForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator);
        IStructureBuilder ForUpdates(IStructureSchema structureSchema);
    }
}