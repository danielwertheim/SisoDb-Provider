using System;
using System.Collections.Generic;
using System.Dynamic;

namespace SisoDb.Dynamic
{
    [Serializable]
    public class DynamicStructure : DynamicObject
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
            var parsedValue = ParseValue(value);
            _memberStates[member] = parsedValue;
            TypeDescriptor.Set(member, parsedValue.GetType());
        }

        private static dynamic ParseValue(dynamic value)
        {
            if (value == null)
                return null;

            //if (value is JToken)
            //{
            //    if (value is JValue)
            //        value = ((JValue)value).Value;
            //    else if (value is JObject)
            //    {
            //        var kv = new Dictionary<string, object>();
            //        var jObj = value as JObject;
            //        foreach (var x in jObj)
            //        {
            //            var v = ParseValue(x.Value);
            //            kv.Add(x.Key, v);
            //        }
            //        dynamic d = new DynamicStructure(kv);
            //        value = d;
            //    }
            //}

            return value;
        }
    }
}