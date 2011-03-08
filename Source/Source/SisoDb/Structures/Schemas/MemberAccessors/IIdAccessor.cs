using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface IIdAccessor : IMemberAccessor
    {
        IdTypes IdType { get; }

        TOut? GetValue<T, TOut>(T item)
            where T : class
            where TOut : struct;

        void SetValue<T, TIn>(T item, TIn value)
            where T : class
            where TIn : struct;
    }
}