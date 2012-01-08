using System;

namespace SisoDb
{
	[Serializable]
	public class Text
	{
		private readonly string _value;

		internal Text(string value)
		{
			_value = value;
		}

		public static implicit operator Text (string value)
		{
			return new Text(value);
		}

		public static implicit operator string (Text item)
		{
			return item._value;
		}

		public override string ToString()
		{
			return _value;
		}
	}
}