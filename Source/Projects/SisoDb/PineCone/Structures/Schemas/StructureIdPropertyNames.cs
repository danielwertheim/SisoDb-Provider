using System;

namespace SisoDb.PineCone.Structures.Schemas
{
    public static class StructureIdPropertyNames
	{
        public const string Indicator = "Id";

	    private enum Names
		{
			StructureId = 0,
			TypeNameSuffixedWithId = 1,
			Id = 2
		}

		public static readonly string Default;
		public static readonly string[] NamesInEvaluationOrder;
			
		static StructureIdPropertyNames()
		{
			Default = Names.StructureId.ToString();

			NamesInEvaluationOrder = new[]
			{
				Names.StructureId.ToString(),
				Names.TypeNameSuffixedWithId.ToString(),
				Names.Id.ToString()
			};
		}

		public static string GetTypeNamePropertyNameFor(Type type)
		{
			return type.Name + Indicator;
		}

		public static string GetInterfaceTypeNamePropertyNameFor(Type type)
		{
			return type.Name.StartsWith("I")
                ? type.Name.Substring(1) + Indicator
                : type.Name + Indicator;
		}
	}
}