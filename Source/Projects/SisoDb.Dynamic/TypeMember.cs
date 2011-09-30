using System;

namespace SisoDb.Dynamic
{
    [Serializable]
    public class TypeMember
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public TypeMember(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}