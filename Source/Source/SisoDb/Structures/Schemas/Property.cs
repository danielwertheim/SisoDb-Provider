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
    public class Property : IProperty
    {
        private static readonly Type UniqueAttributeType = typeof(UniqueAttribute);

        private List<PropertyInfo> Callstack { get; set; }

        public PropertyInfo Member { get; set; }

        public string Name
        {
            get { return Member.Name; }
        }

        public string Path { get; private set; }

        public Type PropertyType
        {
            get { return Member.PropertyType; }
        }

        public int Level { get; private set; }

        public IProperty Parent { get; private set; }

        public bool IsSimpleType { get; private set; }

        public bool IsUnique
        {
            get { return UniqueMode.HasValue; }
        }

        public UniqueModes? UniqueMode { get; private set; }

        public bool IsEnumerable { get; private set; }

        public bool IsElement { get; private set; }

        public Type ElementType { get; private set; }

        public Property(PropertyInfo member)
            : this(0, null, member)
        {
            Member = member;
        }

        public Property(int level, IProperty parent, PropertyInfo member)
        {
            Member = member;
            Level = level;
            Parent = parent;

            IsSimpleType = Member.PropertyType.IsSimpleType();
            IsEnumerable = Member.PropertyType.IsEnumerableType();
            ElementType = IsEnumerable ? Member.PropertyType.GetEnumerableElementType() : null;
            IsElement = Parent != null && (Parent.IsElement || Parent.IsEnumerable);

            var uniqueAttribute = (UniqueAttribute)Member.GetCustomAttributes(UniqueAttributeType, true).FirstOrDefault();
            if (uniqueAttribute != null && !IsSimpleType)
                throw new SisoDbException(ExceptionMessages.Property_Ctor_UniqueOnNonSimpleType);

            UniqueMode = uniqueAttribute == null ? (UniqueModes?)null : uniqueAttribute.Mode;

            Path = PropertyPathBuilder.BuildPath(this);

            Callstack = GetCallstack(this);
            Callstack.Reverse();
        }

        private static List<PropertyInfo> GetCallstack(IProperty property)
        {
            if (property.Level == 0)
                return new List<PropertyInfo> { property.Member };

            var props = new List<PropertyInfo> { property.Member };
            var tmp = GetCallstack(property.Parent);
            props.AddRange(tmp);

            return props;
        }

        public TReturn? GetIdValue<T, TReturn>(T item)
            where T : class
            where TReturn : struct
        {
            if (Level != 0)
                throw new SisoDbException(ExceptionMessages.Property_GetIdValue_InvalidLevel);

            var value = Member.GetValue(item, null);
            if (value == null)
                return null;

            if (value is Guid || value is Guid?)
                return (TReturn)value;

            if (value is int || value is int?)
                return (TReturn)value;

            throw new SisoDbException(ExceptionMessages.Property_GetIdValue_UnsupportedIdDataType);
        }

        public IList<object> GetValues<T>(T item) where T : class
        {
            if (Level == 0)
            {
                var firstLevelPropValue = Member.GetValue(item, null);
                if (firstLevelPropValue == null)
                    return null;

                if (!IsEnumerable)
                    return new List<object> { firstLevelPropValue };

                var values = new List<object>();
                foreach (var value in (ICollection)firstLevelPropValue)
                    values.Add(value);

                return values;
            }

            return TraverseCallstack(item, 0);
        }

        private IList<object> TraverseCallstack<T>(T startNode, int startIndex)
        {
            object currentNode = startNode;
            for (var c = startIndex; c < Callstack.Count; c++)
            {
                if (currentNode == null)
                    return new object[] { null };

                var currentPropertyInfo = Callstack[c];
                var isLastPropertyInfo = c == (Callstack.Count - 1);
                if (isLastPropertyInfo)
                {
                    if (!(currentNode is ICollection))
                    {
                        var currentValue = currentPropertyInfo.GetValue(currentNode, null);
                        return new[] { currentValue };
                    }

                    var currentNodes = (ICollection)currentNode;
                    return ExtractValuesForEnumerableOfComplex(currentNodes, currentPropertyInfo);
                }

                if (!(currentNode is ICollection))
                    currentNode = currentPropertyInfo.GetValue(currentNode, null);
                else
                {
                    var currentNodes = (ICollection)currentNode;
                    var values = new List<object>();
                    foreach (var node in currentNodes)
                    {
                        var nodeValue = currentPropertyInfo.GetValue(node, null);
                        var tmp = TraverseCallstack(nodeValue, c + 1);
                        values.AddRange(tmp);
                    }
                    return values;
                }
            }

            return null;
        }

        private static IList<object> ExtractValuesForEnumerableOfComplex(ICollection nodes, PropertyInfo propertyAccessor)
        {
            var values = new List<object>();
            foreach (var node in nodes)
            {
                if (node == null)
                {
                    values.Add(null);
                    continue;
                }

                var nodeValue = propertyAccessor.GetValue(node, null);

                if(nodeValue == null ||  !(nodeValue is ICollection))
                    values.Add(nodeValue);
                else
                    foreach (var nodeValueElement in (ICollection) nodeValue)
                        values.Add(nodeValueElement);
            }

            return values;
        }

        public void SetValue<TItem, TValue>(TItem item, TValue value) where TItem : class
        {
            if (Level == 0)
            {
                Member.SetValue(item, value, null);
                return;
            }

            object v = item;
            for (var c = 0; c < Callstack.Count; c++)
            {
                var property = Callstack[c];
                var isLastItem = c == Callstack.Count - 1;
                if (isLastItem)
                    property.SetValue(v, value, null);

                v = property.GetValue(v, null);
            }
        }
    }
}