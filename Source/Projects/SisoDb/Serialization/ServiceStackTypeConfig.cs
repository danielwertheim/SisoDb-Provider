using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Serialization
{
    internal static class ServiceStackTypeConfig<T> where T : class
    {
        private static readonly Type StructureContractType;
        private static readonly Type TypeConfigType;
        private static readonly IStructureTypeReflecter StructureTypeReflecter;
        private static readonly ConcurrentDictionary<Type, bool> PreviousConfigurations;

        static ServiceStackTypeConfig()
        {
            StructureContractType = typeof(T);
            TypeConfigType = typeof(TypeConfig<>);
            StructureTypeReflecter = new StructureTypeReflecter();
            PreviousConfigurations = new ConcurrentDictionary<Type, bool>();
            TypeConfig<T>.Properties = ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
        }

        internal static void ExcludeReferencedStructuresFor(Type itemType)
        {
            var propertiesAllreadyExcludedInStaticCtor = itemType == StructureContractType;
            if (propertiesAllreadyExcludedInStaticCtor)
                return;

            if (PreviousConfigurations.ContainsKey(itemType))
                return;

            if (PreviousConfigurations.TryAdd(itemType, true))
                ConfigureTypeConfigToExcludeReferencedStructures(itemType);
        }

        private static void ConfigureTypeConfigToExcludeReferencedStructures(Type itemType)
        {
            var propertiesAllreadyExcludedInStaticCtor = itemType == StructureContractType;
            if (propertiesAllreadyExcludedInStaticCtor)
                return;

            var cfg = TypeConfigType.MakeGenericType(itemType);
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