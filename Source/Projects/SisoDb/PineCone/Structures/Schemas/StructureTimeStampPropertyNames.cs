using System;

namespace SisoDb.PineCone.Structures.Schemas
{
    public static class StructureTimeStampPropertyNames
    {
        public const string Indicator = "TimeStamp";

        private enum Names
        {
            StructureTimeStamp = 0,
            TypeNameSuffixedWithTimeStamp = 1,
            TimeStamp = 2
        }

        public static readonly string Default;
        public static readonly string[] NamesInEvaluationOrder;

        static StructureTimeStampPropertyNames()
        {
            Default = Names.StructureTimeStamp.ToString();

            NamesInEvaluationOrder = new[]
            {
                Names.StructureTimeStamp.ToString(),
                Names.TypeNameSuffixedWithTimeStamp.ToString(),
                Names.TimeStamp.ToString()
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