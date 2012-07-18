using System;

namespace SisoDb.EnsureThat
{
    public class TypeParam : Param
    {
        public readonly Type Type;

        internal TypeParam(string name, Type type)
            : base(name)
        {
            Type = type;
        }
    }
}