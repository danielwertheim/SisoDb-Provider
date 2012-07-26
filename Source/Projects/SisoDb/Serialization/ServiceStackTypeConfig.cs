using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Serialization
{
    internal static class ServiceStackTypeConfig<T>
    {
        private static readonly Type StructureContractType;
        private static readonly Type TypeConfigType;
        private static readonly ConcurrentDictionary<Type, bool> PreviousConfigurations;

        static ServiceStackTypeConfig()
        {
            PreviousConfigurations = new ConcurrentDictionary<Type, bool>();

            StructureContractType = typeof(T);
            TypeConfigType = typeof(TypeConfig<>);

            TypeConfig<T>.Properties = ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
        }

        internal static void Config(Type itemType)
        {
            var propertiesAllreadyExcludedInStaticCtor = itemType == StructureContractType;
            if (propertiesAllreadyExcludedInStaticCtor)
                return;

            if (PreviousConfigurations.ContainsKey(itemType))
                return;

            if (PreviousConfigurations.TryAdd(itemType, true))
                ConfigureTypeConfigToExcludeReferencedStructures(itemType);
        }

        private static void ConfigureTypeConfigToExcludeReferencedStructures(Type type)
        {
            var propertiesAllreadyExcludedInStaticCtor = type == StructureContractType;
            if (propertiesAllreadyExcludedInStaticCtor)
                return;

            var cfg = TypeConfigType.MakeGenericType(type);
            var propertiesProperty = cfg.GetProperty("Properties", BindingFlags.Static | BindingFlags.Public);
            propertiesProperty.SetValue(
                null,
                ExcludePropertiesThatHoldStructures((PropertyInfo[])propertiesProperty.GetValue(null, new object[] { })),
                new object[] { });
        }

        private static PropertyInfo[] ExcludePropertiesThatHoldStructures(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => !StructureTypeReflecter.HasIdProperty(p.PropertyType)).ToArray();
        }
    }
}