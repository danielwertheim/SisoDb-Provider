using System;
using PineCone.Annotations;

namespace PineCone.Structures.Schemas
{
    public interface IStructureProperty
    {
        string Name { get; }
        string Path { get; }
        Type DataType { get; }
        IStructureProperty Parent { get; }
        bool IsRootMember { get; }
        bool IsUnique { get; }
        UniqueModes? UniqueMode { get; }
        bool IsEnumerable { get; }
        bool IsElement { get; }
        Type ElementDataType { get; }
        bool IsReadOnly { get; }
        object GetValue(object item);
        void SetValue(object target, object value);
    }
}