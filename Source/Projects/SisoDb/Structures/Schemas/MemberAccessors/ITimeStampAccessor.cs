using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public interface ITimeStampAccessor : IMemberAccessor
    {
        DateTime? GetValue<T>(T item) where T : class;

        void SetValue<T>(T item, DateTime value) where T : class;
    }
}