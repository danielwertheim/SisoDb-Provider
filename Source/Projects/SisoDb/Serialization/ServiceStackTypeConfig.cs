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
            StructureContractType = typeof(T);
            TypeConfigType = typeof(TypeConfig<>);
            PreviousConfigurations = new ConcurrentDictionary<Type, bool>();
            //TypeConfig<T>.Properties = ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
        }

        internal static void Config(IStructureSchema structureSchema, Type itemType)
        {
            if(!structureSchema.Type.ContainedStructureProperties.Any())
                return;
            
            //var propertiesAllreadyExcludedInStaticCtor = itemType == StructureContractType;
            //if (propertiesAllreadyExcludedInStaticCtor)
            //    return;

            if (PreviousConfigurations.ContainsKey(itemType))
                return;

            if (PreviousConfigurations.TryAdd(itemType, true))
                ConfigureTypeConfigToExcludeReferencedStructures(structureSchema, itemType);
        }

        private static void ConfigureTypeConfigToExcludeReferencedStructures(IStructureSchema structureSchema, Type itemType)
        {
            if (!structureSchema.Type.ContainedStructureProperties.Any())
                return;

            //var propertiesAllreadyExcludedInStaticCtor = itemType == StructureContractType;
            //if (propertiesAllreadyExcludedInStaticCtor)
            //    return;

            var cfg = TypeConfigType.MakeGenericType(itemType);
            var propertiesProperty = cfg.GetProperty("Properties", BindingFlags.Static | BindingFlags.Public);
            propertiesProperty.SetValue(
                null,
                ExcludePropertiesThatHoldStructures(structureSchema, (PropertyInfo[])propertiesProperty.GetValue(null, new object[] { })),
                new object[] { });
        }

        private static PropertyInfo[] ExcludePropertiesThatHoldStructures(IStructureSchema structureSchema, IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => !structureSchema.Type.ContainedStructureProperties.Any(cp => cp.Name == p.Name)).ToArray();
        }
    }
}