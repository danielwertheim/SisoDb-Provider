using System;

namespace SisoDb.Querying
{
    [Serializable]
    public class Paging : IEquatable<Paging>
    {
        public readonly int PageIndex;
        public readonly int PageSize;

        public Paging(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex.AssertGte(0, "pageIndex");
            PageSize = pageSize.AssertGt(0, "pageSize");
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Paging);
        }

        public bool Equals(Paging other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.PageIndex == PageIndex && other.PageSize == PageSize;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PageIndex*397) ^ PageSize;
            }
        }
    }
}