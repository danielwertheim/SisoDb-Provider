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
        private static readonly ConcurrentDictionary<Type, bool> PreviousConfigurations;

        static ServiceStackTypeConfig()
        {
            StructureContractType = typeof(T);
            PreviousConfigurations = new ConcurrentDictionary<Type, bool>();
            TypeConfig<T>.Properties = TypePropertiesFilter.ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
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

            var cfg = new ServiceStackTypeConfig(itemType);
            cfg.SetProperties(TypePropertiesFilter.ExcludePropertiesThatHoldStructures(cfg.GetProperties()));
        }
    }

    internal static class TypePropertiesFilter
    {
        private static readonly IStructureTypeReflecter StructureTypeReflecter = new StructureTypeReflecter();

        internal static PropertyInfo[] ExcludePropertiesThatHoldStructures(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => !StructureTypeReflecter.HasIdProperty(p.PropertyType)).ToArray();
        }
    }

    internal class ServiceStackTypeConfig
    {
        private static readonly Type TypeConfigType = typeof(TypeConfig<>);
        private readonly PropertyInfo _propertiesProperty;

        internal ServiceStackTypeConfig(Type itemType)
        {
            var cfg = TypeConfigType.MakeGenericType(itemType);
            _propertiesProperty = cfg.GetProperty("Properties", BindingFlags.Static | BindingFlags.Public);
        }

        internal void SetProperties(PropertyInfo[] properties)
        {
            _propertiesProperty.SetValue(
                null,
                properties,
                new object[] { });
        }

        internal PropertyInfo[] GetProperties()
        {
            return _propertiesProperty.GetValue(null, new object[] { }) as PropertyInfo[];
        }
    }
}