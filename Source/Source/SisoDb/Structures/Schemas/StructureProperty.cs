using System;
using System.Linq;
using System.Reflection;
using SisoDb.Annotations;
using SisoDb.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures.Schemas
{
    [Serializable]
    public class StructureProperty : IStructureProperty
    {
        private static readonly Type UniqueAttributeType = typeof(UniqueAttribute);

        private readonly DynamicGetter _getter;
        private readonly DynamicSetter _setter;

        public string Name { get; private set; }

        public string Path { get; private set; }

        public Type PropertyType { get; private set; }

        public IStructureProperty Parent { get; private set; }

        public bool IsRootMember { get; private set; }

        public bool IsUnique { get; private set; }

        public StructureIndexUniques Uniqueness { get; private set; }

        public bool IsEnumerable { get; private set; }

        public bool IsElement { get; private set; }

        public Type ElementType { get; private set; }

        public StructureProperty(PropertyInfo propertyInfo)
            : this(null, propertyInfo)
        {
        }

        public StructureProperty(IStructureProperty parent, PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            PropertyType = propertyInfo.PropertyType;
            Parent = parent;

            var isSimpleType = propertyInfo.PropertyType.IsSimpleType();

            IsRootMember = parent == null;
            IsEnumerable = !isSimpleType && propertyInfo.PropertyType.IsEnumerableType();
            ElementType = IsEnumerable ? propertyInfo.PropertyType.GetEnumerableElementType() : null;
            IsElement = Parent != null && (Parent.IsElement || Parent.IsEnumerable);

            var uniqueAttribute = (UniqueAttribute)propertyInfo.GetCustomAttributes(UniqueAttributeType, true).FirstOrDefault();
            if (uniqueAttribute != null && !isSimpleType)
                throw new SisoDbException(ExceptionMessages.Property_Ctor_UniqueOnNonSimpleType);

            Uniqueness = uniqueAttribute == null ? 
                StructureIndexUniques.None : 
                uniqueAttribute.Mode.ToStructureIndexUniques();
            IsUnique = Uniqueness != StructureIndexUniques.None;

            Path = PropertyPathBuilder.BuildPath(this);

            _getter = DynamicPropertyFactory.CreateGetter(propertyInfo);
            _setter = DynamicPropertyFactory.CreateSetter(propertyInfo);
        }

        public object GetValue(object item)
        {
            return _getter(item);
        }

        public void SetValue(object target, object value)
        {
            _setter(target, value);
        }
    }
}