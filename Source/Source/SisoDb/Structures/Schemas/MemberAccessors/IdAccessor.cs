using System;
using SisoDb.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IdAccessor : MemberAccessorBase, IIdAccessor
    {
        public IdTypes IdType { get; private set; }

        public TReturn? GetValue<T, TReturn>(T item) where T : class where TReturn : struct 
        {
            return Property.GetIdValue<T, TReturn>(item);
        }

        public void SetValue<T>(T item, ValueType value) where T : class
        {
            Property.SetValue(item, value);
        }

        public IdAccessor(IProperty property)
            : base(property)
        {
            if (Property.PropertyType.IsGuidType() || Property.PropertyType.IsNullableGuidType())
                IdType = IdTypes.Guid;
            else if (Property.PropertyType.IsIntType() || Property.PropertyType.IsNullableIntType())
                IdType = IdTypes.Identity;
            else
                throw new SisoDbException(
                    ExceptionMessages.IdAccessor_UnsupportedPropertyType.Inject(property.PropertyType.Name));
        }
    }
}