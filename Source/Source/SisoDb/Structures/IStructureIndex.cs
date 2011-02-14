namespace SisoDb.Structures
{
    public interface IStructureIndex
    {
        IStructureId StructureId { get;  }

        string Name { get;  }
        
        object Value { get; }

        bool IsUnique { get; }
    }
}