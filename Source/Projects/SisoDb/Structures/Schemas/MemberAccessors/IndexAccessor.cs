using System.Collections;
using System.Collections.Generic;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IndexAccessor : MemberAccessorBase, IIndexAccessor
    {
        private readonly List<IStructureProperty> _callstack;

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

        public StructureIndexType IndexType
        {
            get { return Property.IndexType; }
        }

        public IndexAccessor(IStructureProperty property) 
            : base(property)
        {
            _callstack = GetCallstack(Property);
            _callstack.Reverse();
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
                return TraverseCallstack(item, 0);

            var firstLevelPropValue = Property.GetValue(item);
            if (firstLevelPropValue == null)
                return null;

            if (!IsEnumerable)
                return new[] { firstLevelPropValue };

            var values = new List<object>();
            foreach (var value in (ICollection)firstLevelPropValue)
                values.Add(value);

            return values;
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
                        return new[] { currentProperty.GetValue(currentNode) };

                    return ExtractValuesForEnumerableOfComplex(
                        (ICollection)currentNode,
                        currentProperty);
                }

                if (!(currentNode is ICollection))
                    currentNode = currentProperty.GetValue(currentNode);
                else
                {
                    var values = new List<object>();
                    foreach (var node in (ICollection)currentNode)
                    {
                        values.AddRange(
                            TraverseCallstack(
                            currentProperty.GetValue(node),
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

                var nodeValue = property.GetValue(node);

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