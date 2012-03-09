using System;

namespace SisoDb
{
	[Serializable]
	public struct Text : IEquatable<Text>, IEquatable<string>
	{
		private readonly string _value;

		public Text(string value)
		{
			_value = value;
		}

		public static implicit operator Text(string value)
		{
			return new Text(value);
		}

		public static implicit operator string(Text item)
		{
			return item._value;
		}

	    public bool Equals(string other)
	    {
	        return Equals(_value, other);
	    }

	    public override string ToString()
		{
			return _value;
		}

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Text)) return false;
            return Equals((Text)obj);
        }

	    public bool Equals(Text other)
	    {
	        return Equals(other._value, _value);
	    }

	    public override int GetHashCode()
	    {
	        return (_value != null ? _value.GetHashCode() : 0);
	    }

	    public static bool operator ==(Text left, Text right)
	    {
	        return left.Equals(right);
	    }

	    public static bool operator !=(Text left, Text right)
	    {
	        return !left.Equals(right);
	    }

        public static bool operator ==(Text left, string right)
        {
            return Equals(left._value, right);
        }

        public static bool operator !=(Text left, string right)
        {
            return !Equals(left._value, right);
        }
	}
}