using System;
using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        Func<IStructureSchema, IStructureBuilder> ForInserts { get; set; }
        Func<IStructureSchema, IStructureBuilder> ForUpdates { get; set; }
    }
}