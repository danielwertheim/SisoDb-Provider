using System.Collections.Generic;

namespace SisoDb.Structures
{
    public interface IStructure
    {
        ISisoId Id { get; }

        //byte[] Version { get; }

        string Name { get; }
        
        string Json { get; }
        
        ISet<IStructureIndex> Indexes { get; }

        ISet<IStructureIndex> Uniques { get; }
    }
}