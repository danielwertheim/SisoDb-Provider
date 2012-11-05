using System;
using SisoDb.Dac;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        Func<IStructureSchema, IStructureIdGenerator> GuidStructureIdGeneratorFn { get; set; }
        Func<IStructureSchema, IDbClient, IIdentityStructureIdGenerator> IdentityStructureIdGeneratorFn { get; set; }
        Func<IStructureSchema, IDbClient, IStructureBuilder> ForInserts { get; set; }
        Func<IStructureSchema, IStructureBuilder> ForUpdates { get; set; }
    }
}