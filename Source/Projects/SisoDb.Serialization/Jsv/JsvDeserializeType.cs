using System;
using System.Reflection;
using SisoDb.Serialization.Common;

namespace SisoDb.Serialization.Jsv
{
	public static class JsvDeserializeType
	{
		public static SetPropertyDelegate GetSetPropertyMethod(Type type, PropertyInfo propertyInfo)
		{
			return TypeAccessor.GetSetPropertyMethod(type, propertyInfo);
		}
	}
}