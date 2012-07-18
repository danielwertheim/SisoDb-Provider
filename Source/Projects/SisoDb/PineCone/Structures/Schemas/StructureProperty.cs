using System;
using NCore;
using NCore.Reflections;
using PineCone.Annotations;
using PineCone.Resources;

namespace PineCone.Structures.Schemas
{
    [Serializable]
    public class StructureProperty : IStructureProperty
    {
        private readonly DynamicGetter _getter;
        private readonly DynamicSetter _setter;

        public string Name { get; private set; }

        public string Path { get; private set; }

        public Type DataType { get; private set; }

        public IStructureProperty Parent { get; private set; }

        public bool IsRootMember { get; private set; }

        public bool IsUnique
        {
            get { return UniqueMode.HasValue; }
        }

        public UniqueModes? UniqueMode { get; private set; }

        public bool IsEnumerable { get; private set; }

        public bool IsElement { get; private set; }

        public Type ElementDataType { get; private set; }

        public bool IsReadOnly { get; private set; }

        public StructureProperty(StructurePropertyInfo info, DynamicGetter getter, DynamicSetter setter = null)
        {
            _getter = getter;
            _setter = setter;

            Parent = info.Parent;
            Name = info.Name;
            DataType = info.DataType;
            IsRootMember = info.Parent == null;
            IsReadOnly = _setter == null;
            UniqueMode = info.UniqueMode;

            var isSimpleOrValueType = DataType.IsSimpleType() || DataType.IsValueType;
            IsEnumerable = !isSimpleOrValueType && DataType.IsEnumerableType();
            IsElement = Parent != null && (Parent.IsElement || Parent.IsEnumerable);
            ElementDataType = IsEnumerable ? DataType.GetEnumerableElementType() : null;

            if (IsUnique && !isSimpleOrValueType)
                throw new PineConeException(ExceptionMessages.StructureProperty_Ctor_UniqueOnNonSimpleType);

            Path = PropertyPathBuilder.BuildPath(this);
        }

        public virtual object GetValue(object item)
        {
            return _getter.GetValue(item);
        }

        public virtual void SetValue(object target, object value)
        {
            if (IsReadOnly)
                throw new PineConeException(ExceptionMessages.StructureProperty_Setter_IsReadOnly.Inject(Path));

            _setter.SetValue(target, value);
        }
    }
}