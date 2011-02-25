using System.Collections.Generic;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public class IndexAccessor : MemberAccessorBase, IIndexAccessor
    {
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

        public IndexAccessor(IProperty property) : base(property)
        {
        }

        public IList<object> GetValues<T>(T item) where T : class
        {
            return Property.GetValues(item);
        }
    }
}