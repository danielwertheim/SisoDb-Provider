using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace SisoDb.Dynamic
{
    [Serializable]
    public class DynamicStructure : DynamicObject, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, dynamic> _memberStates;
        
        public TypeDescriptor TypeDescriptor { get; private set; }

        public DynamicStructure()
        {
            _memberStates = new Dictionary<string, dynamic>();
            TypeDescriptor = new TypeDescriptor();
        }

        public DynamicStructure(IEnumerable<KeyValuePair<string, dynamic>> memberStates) : this()
        {
            VisitState(this, memberStates);
        }

        private static void VisitState(DynamicStructure ds, IEnumerable<KeyValuePair<string, dynamic>> memberStates)
        {
            foreach (var member in memberStates)
            {
                ds.SetValue(member.Key, member.Value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _memberStates.Keys;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetValue(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _memberStates.TryGetValue(binder.Name, out result);
        }

        private void SetValue<T>(string member, T value)
        {
            _memberStates[member] = value;
            TypeDescriptor.Add(member, value.GetType());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, dynamic>> GetEnumerator()
        {
            return _memberStates.GetEnumerator();
        }

        public Dictionary<string, dynamic> ToDictionary()
        {
            return _memberStates;
        }

        public static implicit operator Dictionary<string, dynamic>(DynamicStructure ds)
        {
            return ds.ToDictionary();
        }
    }
}