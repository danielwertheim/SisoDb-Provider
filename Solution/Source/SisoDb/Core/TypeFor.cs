using System;

namespace SisoDb.Core
{
    public static class TypeFor<T> where T : class
    {
        public static readonly Type Type = typeof (T);
    }
}