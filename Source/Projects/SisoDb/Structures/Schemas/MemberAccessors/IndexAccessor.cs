using System.Collections;
using System.Collections.Generic;
using SisoDb.Annotations;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IndexAccessor : MemberAccessorBase, IIndexAccessor
    {
        private delegate void OnLastPropertyFound(IStructureProperty lastProperty, object currentNode);

        private readonly List<IStructureProperty> _callstack;

        public DataTypeCode DataTypeCode { get; private set; }

        public bool IsEnumerable
        {
            get { return Property.IsEnumerable; }
        }

        public bool IsElement
        {
            get { return Property.IsElement; }
        }

        public bool IsUnique
        {
            get { return Property.IsUnique; }
        }

        public UniqueModes? UniqueMode
        {
            get { return Property.UniqueMode; }
        }

        public IndexAccessor(IStructureProperty property, DataTypeCode dataTypeCode)
            : base(property)
        {
            _callstack = GetCallstack(Property);
            _callstack.Reverse();
            DataTypeCode = dataTypeCode;
        }

        private static List<IStructureProperty> GetCallstack(IStructureProperty property)
        {
            if (property.IsRootMember)
                return new List<IStructureProperty> { property };

            var props = new List<IStructureProperty> { property };
            props.AddRange(
                GetCallstack(property.Parent));

            return props;
        }

        public IList<object> GetValues<T>(T item) where T : class
        {
            if (!Property.IsRootMember)
                return EvaluateCallstack(item, startIndex: 0);

            var firstLevelPropValue = Property.GetValue(item);
            if (firstLevelPropValue == null)
                return null;

            return IsEnumerable
                ? CollectionOfValuesToList((IEnumerable)firstLevelPropValue)
                : new[] { firstLevelPropValue };
        }

        private IList<object> EvaluateCallstack<T>(T startNode, int startIndex)
        {
            object currentNode = startNode;
            for (var c = startIndex; c < _callstack.Count; c++)
            {
                if (currentNode == null)
                    return new object[] { null };

                var currentProperty = _callstack[c];
                var isLastProperty = c == (_callstack.Count - 1);
                if (isLastProperty)
                    return currentNode is IEnumerable
                        ? ExtractValuesForEnumerableNode((IEnumerable)currentNode, currentProperty)
                        : ExtractValuesForSimpleNode(currentNode, currentProperty);

                if (!(currentNode is IEnumerable))
                    currentNode = currentProperty.GetValue(currentNode);
                else
                {
                    var values = new List<object>();
                    foreach (var node in (IEnumerable)currentNode)
                        values.AddRange(EvaluateCallstack(currentProperty.GetValue(node), startIndex: c + 1));
                    return values;
                }
            }

            return null;
        }

        private static IList<object> ExtractValuesForEnumerableNode<T>(T nodes, IStructureProperty property) where T : IEnumerable
        {
            var values = nodes is ICollection ? new List<object>(((ICollection)nodes).Count) : new List<object>();

            foreach (var node in nodes)
            {
                if (node == null)
                {
                    values.Add(null);
                    continue;
                }

                var nodeValue = property.GetValue(node);
                if(nodeValue == null)
                {
                    values.Add(null);
                    continue;
                }

                if(nodeValue is IEnumerable && !(nodeValue is string))
                    values.AddRange(CollectionOfValuesToList((IEnumerable)nodeValue));
                else
                    values.Add(nodeValue);
            }

            return values;
        }

        private static IList<object> ExtractValuesForSimpleNode(object node, IStructureProperty property)
        {
            var currentValue = property.GetValue(node);

            if (currentValue == null)
                return null;

            if (!property.IsEnumerable)
                return new[] { currentValue };

            return CollectionOfValuesToList((IEnumerable)currentValue);
        }

        private static IList<object> CollectionOfValuesToList<T>(T elements) where T : IEnumerable
        {
            var values = elements is ICollection ? new List<object>(((ICollection)elements).Count) : new List<object>();
            foreach (var element in elements)
                values.Add(element);

            return values;
        }

        public void SetValue<T>(T item, object value) where T : class
        {
            if (Property.IsRootMember)
            {
                Property.SetValue(item, value);
                return;
            }

            EnumerateToLastProperty(item, startIndex: 0, onLastPropertyFound: (lastProperty, currentNode) => lastProperty.SetValue(currentNode, value));
        }

        private void EnumerateToLastProperty<T>(T startNode, int startIndex, OnLastPropertyFound onLastPropertyFound) where T : class
        {
            object currentNode = startNode;
            for (var c = startIndex; c < _callstack.Count; c++)
            {
                var currentProperty = _callstack[c];
                var isLastPropertyInfo = c == (_callstack.Count - 1);
                if (isLastPropertyInfo)
                {
                    onLastPropertyFound(currentProperty, currentNode);
                    break;
                }

                currentNode = currentProperty.GetValue(currentNode);
            }
        }
    }
}