using System;
using SisoDb.PineCone.Structures;

namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        Func<IStructureIdGenerator> GuidStructureIdGeneratorFn { get; set; }
        StructureBuilderFactoryForInserts ForInserts { get; set; }
        StructureBuilderFactoryForUpdates ForUpdates { get; set; }
    }
}