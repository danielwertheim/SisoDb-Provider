using System.Collections.Generic;

namespace SisoDb.Structures
{
    public interface IStructure
    {
        IStructureId Id { get; }

        //byte[] Version { get; }

        string Name { get; }
        
        string Json { get; }
        
        ISet<IStructureIndex> Indexes { get; }

        ISet<IStructureIndex> Uniques { get; }
    }
}