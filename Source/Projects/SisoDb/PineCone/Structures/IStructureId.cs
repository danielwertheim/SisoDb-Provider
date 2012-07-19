using System;

namespace SisoDb.PineCone.Structures
{
    public interface IStructureId : IEquatable<IStructureId>, IComparable<IStructureId>
    {
        StructureIdTypes IdType { get; }
        object Value { get; }
        Type DataType { get; }
        bool HasValue { get; }
    }
}