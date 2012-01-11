using System;

namespace SisoDb
{
	[Serializable]
	public struct Text
	{
		private readonly string _value;

		public Text(string value)
		{
			_value = value;
		}

		public static Text Parse(string value)
		{
			return value == null ? null : new Text(value);
		}

		public static implicit operator Text(string value)
		{
			return new Text(value);
		}

		public static implicit operator string(Text item)
		{
			return item._value;
		}

		public override string ToString()
		{
			return _value;
		}
	}
}