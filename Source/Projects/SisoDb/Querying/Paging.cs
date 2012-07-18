using System;
using SisoDb.EnsureThat;

namespace SisoDb.Querying
{
    [Serializable]
    public class Paging : IEquatable<Paging>
    {
        public readonly int PageIndex;
        public readonly int PageSize;

        public Paging(int pageIndex, int pageSize)
        {
            Ensure.That(pageIndex, "pageIndex").IsGte(0);
            Ensure.That(pageSize, "pageSize").IsGt(0);
            PageIndex = pageIndex;
            PageSize = pageSize;
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