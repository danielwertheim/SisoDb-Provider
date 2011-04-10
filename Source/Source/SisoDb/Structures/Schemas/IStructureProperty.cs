using System;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureProperty
    {
        string Name { get; }

        string Path { get; }

        Type PropertyType { get; }

        IStructureProperty Parent { get; }

        bool IsRootMember { get; }

        bool IsUnique { get; }
        
        bool IsEnumerable { get; }

        bool IsElement { get; }
        
        Type ElementType { get; }

        object GetValue(object item);

        void SetValue(object target, object value);
    }
}