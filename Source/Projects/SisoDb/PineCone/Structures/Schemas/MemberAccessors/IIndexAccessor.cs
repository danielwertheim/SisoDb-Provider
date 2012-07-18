using System.Collections.Generic;
using PineCone.Annotations;

namespace PineCone.Structures.Schemas.MemberAccessors
{
    public interface IIndexAccessor : IMemberAccessor
    {
        DataTypeCode DataTypeCode { get; }
        bool IsEnumerable { get; }
        bool IsElement { get; }
        bool IsUnique { get; }
        UniqueModes? UniqueMode { get; }
        
        IList<object> GetValues<T>(T item) where T : class;
        void SetValue<T>(T item, object value) where T : class;
    }
}