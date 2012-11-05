using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IMemberAccessor
    {
        string Path { get; }
        Type DataType { get; }
    }
}