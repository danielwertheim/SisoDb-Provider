namespace SisoDb.Structures
{
    public interface IStructureIndex
    {
        ISisoId SisoId { get;  }

        string Name { get;  }
        
        object Value { get; }

        bool IsUnique { get; }

        StructureIndexType IndexType { get; }
    }
}