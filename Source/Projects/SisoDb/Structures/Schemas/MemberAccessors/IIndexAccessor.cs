using System.Collections.Generic;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IIndexAccessor : IMemberAccessor
    {
        bool IsEnumerable { get; }

        bool IsElement { get; }

        bool IsUnique { get; }

        StructureIndexType IndexType { get; }

        IList<object> GetValues<T>(T item) where T : class;
    }
}