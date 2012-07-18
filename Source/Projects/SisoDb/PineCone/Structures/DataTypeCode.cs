using System;

namespace SisoDb.PineCone.Structures
{
	[Serializable]
	public enum DataTypeCode
	{
		Unknown,
		IntegerNumber,
        UnsignedIntegerNumber,
		FractalNumber,
		Bool,
		DateTime,
		Guid,
		String,
        Text,
		Enum
	}
}