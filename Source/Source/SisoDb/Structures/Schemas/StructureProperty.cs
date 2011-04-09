using System;
using System.Collections;
using System.Collections.Generic;
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

        private readonly Prop _prop;
        private readonly List<IStructureProperty> _callstack;

        public string Name { get; private set; }

        public string Path { get; private set; }

        public Type PropertyType { get; private set; }

        public IStructureProperty Parent { get; private set; }

        public bool IsRootMember { get; private set; }

        public bool IsUnique { get; private set; }

        public bool IsEnumerable { get; private set; }

        public bool IsElement { get; private set; }

        public Type ElementType { get; private set; }

        public StructureProperty(PropertyInfo member)
            : this(null, member)
        {
        }

        public StructureProperty(IStructureProperty parent, PropertyInfo member)
        {
            Name = member.Name;
            PropertyType = member.PropertyType;
            Parent = parent;

            var isSimpleType = member.PropertyType.IsSimpleType();

            IsRootMember = parent == null;
            IsEnumerable = !isSimpleType && member.PropertyType.IsEnumerableType();
            ElementType = IsEnumerable ? member.PropertyType.GetEnumerableElementType() : null;
            IsElement = Parent != null && (Parent.IsElement || Parent.IsEnumerable);

            var uniqueAttribute = (UniqueAttribute)member.GetCustomAttributes(UniqueAttributeType, true).FirstOrDefault();
            if (uniqueAttribute != null && !isSimpleType)
                throw new SisoDbException(ExceptionMessages.Property_Ctor_UniqueOnNonSimpleType);
            
            IsUnique = uniqueAttribute != null ? true : false;

            Path = PropertyPathBuilder.BuildPath(this);

            _callstack = GetCallstack(this);
            _callstack.Reverse();

            _prop = new Prop(member);
        }

        private static List<IStructureProperty> GetCallstack(IStructureProperty property)
        {
            if (property.IsRootMember)
                return new List<IStructureProperty> { property};

            var props = new List<IStructureProperty> { property };
            props.AddRange(
                GetCallstack(property.Parent));

            return props;
        }

        public TOut? GetIdValue<TRoot, TOut>(TRoot root)
            where TRoot : class
            where TOut : struct
        {
            if (!IsRootMember)
                throw new SisoDbException(ExceptionMessages.Property_GetIdValue_InvalidLevel);

            return (TOut?)_prop.Getter(root);
        }

        public void SetIdValue<TRoot, TIn>(TRoot root, TIn value)
            where TRoot : class
            where TIn : struct
        {
            if (!IsRootMember)
                throw new SisoDbException(ExceptionMessages.Property_SetIdValue_InvalidLevel);

            _prop.Setter(root, value);
        }

        public IList<object> GetValues<TRoot>(TRoot item)
            where TRoot : class
        {
            if (!IsRootMember)
                return TraverseCallstack(item, 0);

            var firstLevelPropValue = ReflectValue(item);
            if (firstLevelPropValue == null)
                return null;

            if (!IsEnumerable)
                return new [] { firstLevelPropValue };

            var values = new List<object>();
            foreach (var value in (ICollection)firstLevelPropValue)
                values.Add(value);

            return values;
        }

        public object ReflectValue(object item)
        {
            return _prop.Getter(item);
        }

        private IList<object> TraverseCallstack<T>(T startNode, int startIndex)
        {
            object currentNode = startNode;
            for (var c = startIndex; c < _callstack.Count; c++)
            {
                if (currentNode == null)
                    return new object[] { null };

                var currentProperty = _callstack[c];
                var isLastPropertyInfo = c == (_callstack.Count - 1);
                if (isLastPropertyInfo)
                {
                    if (!(currentNode is ICollection))
                        return new[] { currentProperty.ReflectValue(currentNode) };

                    return ExtractValuesForEnumerableOfComplex(
                        (ICollection)currentNode, 
                        currentProperty);
                }

                if (!(currentNode is ICollection))
                    currentNode = currentProperty.ReflectValue(currentNode);
                else
                {
                    var values = new List<object>();
                    foreach (var node in (ICollection)currentNode)
                    {
                        values.AddRange(
                            TraverseCallstack(
                            currentProperty.ReflectValue(node),
                            startIndex: c + 1));
                    }
                    return values;
                }
            }

            return null;
        }

        private static IList<object> ExtractValuesForEnumerableOfComplex(ICollection nodes, IStructureProperty property)
        {
            var values = new List<object>();
            foreach (var node in nodes)
            {
                if (node == null)
                {
                    values.Add(null);
                    continue;
                }

                var nodeValue = property.ReflectValue(node);

                if (nodeValue == null || !(nodeValue is ICollection))
                    values.Add(nodeValue);
                else
                    foreach (var nodeValueElement in (ICollection)nodeValue)
                        values.Add(nodeValueElement);
            }

            return values;
        }
    }
}