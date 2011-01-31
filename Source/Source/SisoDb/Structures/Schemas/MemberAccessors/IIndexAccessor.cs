using System.Collections.Generic;
using SisoDb.Annotations;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IIndexAccessor : IMemberAccessor
    {
        int Level { get; }

        bool IsPrimitive { get; }

        bool IsEnumerable { get; }

        bool IsElement { get; }

        bool IsUnique { get; }

        UniqueModes? Uniqueness { get; }

        IList<object> GetValues<T>(T item) where T : class;
    }
}