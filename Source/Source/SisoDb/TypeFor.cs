using System;

namespace SisoDb
{
    public static class TypeFor<T> where T : class
    {
        public static readonly Type Type = typeof (T);
    }
}