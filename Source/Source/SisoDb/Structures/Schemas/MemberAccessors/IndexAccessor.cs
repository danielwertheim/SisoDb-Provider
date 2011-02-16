using System.Collections.Generic;
using SisoDb.Annotations;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IndexAccessor : MemberAccessorBase, IIndexAccessor
    {
        public int Level
        {
            get { return Property.Level; }
        }

        public bool IsPrimitive
        {
            get { return Property.IsSimpleType; }
        }

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

        public UniqueModes? Uniqueness 
        {
            get { return Property.UniqueMode; }
        }

        public IndexAccessor(IProperty property) : base(property)
        {
        }

        public IList<object> GetValues<T>(T item) where T : class
        {
            return Property.GetValues(item);
        }
    }
}