using System;
using SisoDb.NCore;
using SisoDb.NCore.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class TimeStampAccessor : MemberAccessorBase, ITimeStampAccessor
    {
        public TimeStampAccessor(IStructureProperty property)
            : base(property)
        {
            if (!property.IsRootMember)
                throw new SisoDbException(ExceptionMessages.TimeStampAccessor_InvalidLevel.Inject(Property.Name));

            if (!property.DataType.IsAnyDateTimeType())
                throw new SisoDbException(ExceptionMessages.TimeStampAccessor_Invalid_Type.Inject(Property.Name));
        }

        public DateTime? GetValue<T>(T item) where T : class
        {
            return (DateTime?)Property.GetValue(item);
        }
        
        public void SetValue<T>(T item, DateTime value)
            where T : class
        {
            Property.SetValue(item, value);
        }
    }
}