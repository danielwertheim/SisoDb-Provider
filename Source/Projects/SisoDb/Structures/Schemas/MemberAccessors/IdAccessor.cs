using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IdAccessor : MemberAccessorBase, IIdAccessor
    {
        private readonly StructureIdGetters.IGetter _getter;
        private readonly StructureIdSetters.ISetter _setter;

        public StructureIdTypes IdType { get; private set; }

        public IdAccessor(IStructureProperty property)
            : base(property)
        {
            if (!property.IsRootMember)
                throw new SisoDbException(ExceptionMessages.IdAccessor_InvalidLevel);

            if (!StructureId.IsValidDataType(property.DataType))
                throw new SisoDbException(ExceptionMessages.IdAccessor_UnsupportedPropertyType.Inject(Property.DataType.Name));

            IdType = StructureId.GetIdTypeFrom(property.DataType);

            _getter = StructureIdGetters.For(IdType, Property.DataType);
            _setter = StructureIdSetters.For(IdType, Property.DataType);
        }

        public IStructureId GetValue<T>(T item)
            where T : class
        {
            return _getter.GetIdValue(item, Property);
        }
        
        public void SetValue<T>(T item, IStructureId value)
            where T : class
        {
            _setter.SetIdValue(item, value, Property);
        }
    }
}