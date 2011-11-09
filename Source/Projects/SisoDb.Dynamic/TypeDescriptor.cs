using System;
using System.Collections;
using System.Collections.Generic;

namespace SisoDb.Dynamic
{
    [Serializable]
    public class TypeDescriptor : IEnumerable<TypeMember>
    {
        private readonly Dictionary<string, TypeMember> _state;

        public TypeDescriptor()
        {
            _state = new Dictionary<string, TypeMember>();
        }

        public TypeMember Get(string name)
        {
            return _state.ContainsKey(name) ? _state[name] : null;
        }

        public void Add(string name, Type type)
        {
            _state[name] = new TypeMember(name, type);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TypeMember> GetEnumerator()
        {
            return _state.Values.GetEnumerator();
        }
    }
}