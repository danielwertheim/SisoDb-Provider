using System;

namespace SisoDb.Querying
{
    [Serializable]
    public class QueryParameter : IQueryParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        
        public QueryParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Name = name;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as QueryParameter);
        }

        public bool Equals(IQueryParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}