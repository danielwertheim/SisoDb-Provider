using System;
using SisoDb.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IdAccessor : MemberAccessorBase, IIdAccessor
    {
        public IdTypes IdType { get; private set; }

        public TReturn? GetValue<TReturn>(object item) where TReturn : struct 
        {
            return Property.GetIdValue<TReturn>(item);
        }

        public void SetValue(object item, ValueType value)
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