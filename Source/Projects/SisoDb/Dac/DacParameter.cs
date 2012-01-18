using System;

namespace SisoDb.Dac
{
    [Serializable]
    public class DacParameter : IDacParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        
        public DacParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Name = name;
            Value = value ?? DBNull.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DacParameter);
        }

        public bool Equals(IDacParameter other)
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