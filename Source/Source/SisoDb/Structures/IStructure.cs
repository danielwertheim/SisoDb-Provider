using System.Collections.Generic;

namespace SisoDb.Structures
{
    internal interface IStructure
    {
        IStructureId Id { get; }

        //byte[] Version { get; }

        string TypeName { get; }
        
        string Json { get; }
        
        ISet<IStructureIndex> Indexes { get; }

        ISet<IStructureIndex> Uniques { get; }
    }
}