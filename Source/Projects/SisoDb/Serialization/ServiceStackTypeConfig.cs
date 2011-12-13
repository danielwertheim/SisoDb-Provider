using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PineCone.Structures.Schemas;
using ServiceStack.Text;

namespace SisoDb.Serialization
{
    internal static class ServiceStackTypeConfig<T> where T : class
    {
    	private static readonly IStructureTypeReflecter StructureTypeReflecter;
        private static readonly Type TypeConfig = typeof(TypeConfig<>);
        private static Action<Type> _config;
         
        static ServiceStackTypeConfig()
        {
			StructureTypeReflecter = new StructureTypeReflecter();
            JsConfig<T>.ExcludeTypeInfo = true;
            _config = RealConfig;
        }
 
        internal static void Config(Type type)
        {
            _config.Invoke(type);
        }

        internal static void FakeConfig(Type type) {}

        internal static void RealConfig(Type type)
        {
            lock(TypeConfig)
            {
                var cfg = TypeConfig.MakeGenericType(type);
                var propertiesField = cfg.GetField("Properties", BindingFlags.Static | BindingFlags.Public);
                propertiesField.SetValue(null, ExcludePropertiesThatHoldStructures((PropertyInfo[])propertiesField.GetValue(null)));
                
                _config = FakeConfig;
            }
        }

        private static PropertyInfo[] ExcludePropertiesThatHoldStructures(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => !StructureTypeReflecter.HasIdProperty(p.PropertyType)).ToArray();
        }
    }
}