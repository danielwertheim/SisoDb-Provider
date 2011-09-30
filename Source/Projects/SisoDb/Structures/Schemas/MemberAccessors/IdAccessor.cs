using System;
using SisoDb.Core;
using SisoDb.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IdAccessor : MemberAccessorBase, IIdAccessor
    {
        public IdTypes IdType { get; private set; }

        public IdAccessor(IStructureProperty property)
            : base(property)
        {
            if (!property.IsRootMember)
                throw new SisoDbException(ExceptionMessages.IdAccessor_GetIdValue_InvalidLevel);

            if (Property.PropertyType.IsGuidType() || Property.PropertyType.IsNullableGuidType())
                IdType = IdTypes.Guid;
            else if (Property.PropertyType.IsIntType() || Property.PropertyType.IsNullableIntType())
                IdType = IdTypes.Identity;
            else
                throw new SisoDbException(
                    ExceptionMessages.IdAccessor_UnsupportedPropertyType.Inject(Property.PropertyType.Name));
        }

        public TOut? GetValue<T, TOut>(T item)
            where T : class
            where TOut : struct
        {
            return (TOut?)Property.GetValue(item);
        }
        
        public void SetValue<T>(T item, ValueType value)
            where T : class
        {
            Property.SetValue(item, value);
        }
    }
}