using System.Collections.Generic;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public interface IDbPipe
    {
        string Reading(IStructureSchema structureSchema, string data);
        IEnumerable<string> Reading(IStructureSchema structureSchema, IEnumerable<string> data);

        IStructure Writing(IStructureSchema structureSchema, IStructure structure);
        IEnumerable<IStructure> Writing(IStructureSchema structureSchema, IEnumerable<IStructure> data);
    }
}