using System;

namespace SisoDb
{
    internal static class TypeFor<T> where T : class
    {
        internal static readonly Type Type = typeof (T);
    }
}