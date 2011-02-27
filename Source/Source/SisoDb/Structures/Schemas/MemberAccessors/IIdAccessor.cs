using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IIdAccessor : IMemberAccessor
    {
        IdTypes IdType { get; }

        TReturn? GetValue<TReturn>(object item)
            where TReturn : struct;

        void SetValue(object item, ValueType value);
    }
}