namespace SisoDb.Structures
{
    internal interface IStructureIndex
    {
        IStructureId StructureId { get;  }

        string Name { get;  }
        
        object Value { get; }

        bool IsUnique { get; }
    }
}