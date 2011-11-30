using System;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        Func<IStructureSchema, GetNextIdentity, IStructureBuilder> ForInserts { get; set; }
        Func<IStructureSchema, IStructureBuilder> ForUpdates { get; set; }
    }
}