using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IIdAccessor : IMemberAccessor
    {
        IdTypes IdType { get; }

        TReturn? GetValue<T, TReturn>(T item)
            where T : class
            where TReturn : struct;

        void SetValue<T>(T item, ValueType value) where T : class;
    }
}