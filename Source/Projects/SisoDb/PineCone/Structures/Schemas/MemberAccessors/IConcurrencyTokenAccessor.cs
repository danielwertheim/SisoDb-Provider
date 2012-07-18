using System;

namespace PineCone.Structures.Schemas.MemberAccessors
{
    public interface IConcurrencyTokenAccessor : IMemberAccessor
    {
        object GetValue<T>(T item) where T : class;

        void SetValue<T>(T item, Guid value) where T : class;

        void SetValue<T>(T item, int value) where T : class;

        void SetValue<T>(T item, long value) where T : class;
    }
}