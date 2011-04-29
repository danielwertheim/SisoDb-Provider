using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IMemberAccessor
    {
        string Name { get; }

        string Path { get; }

        Type DataType { get; }
    }
}