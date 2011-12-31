using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PineCone.Structures.Schemas;
using ServiceStack.Text;

namespace SisoDb.Serialization
{
    internal static class ServiceStackTypeConfig<T>
	{
		private static readonly IStructureTypeReflecter StructureTypeReflecter;

		private static readonly Type ItemType;

		private static readonly Type TypeConfigType;

		private static readonly ConcurrentDictionary<Type, Action> ConfigActions;

		internal static void Config(Type type)
		{
			if (type == ItemType)
				return;

			lock (ConfigActions)
			{
				ConfigActions.GetOrAdd(type, t =>
				{
					Action a = () => { };

					ConfigureTypeConfigToExcludeReferencedStructures(t);

					return a;
				}).Invoke();
			}
		}

		static ServiceStackTypeConfig()
		{
			ConfigActions = new ConcurrentDictionary<Type, Action>();

			StructureTypeReflecter = new StructureTypeReflecter();
			ItemType = typeof(T);
			TypeConfigType = typeof(TypeConfig<>);

			TypeConfig<T>.Properties = ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
			JsConfig<T>.ExcludeTypeInfo = true;	
		}

		private static void ConfigureTypeConfigToExcludeReferencedStructures(Type type)
		{
			if(type == ItemType)
				return;

			var cfg = TypeConfigType.MakeGenericType(type);
			var propertiesField = cfg.GetField("Properties", BindingFlags.Static | BindingFlags.Public);
			propertiesField.SetValue(null, ExcludePropertiesThatHoldStructures((PropertyInfo[])propertiesField.GetValue(null)));
		}

		private static PropertyInfo[] ExcludePropertiesThatHoldStructures(IEnumerable<PropertyInfo> properties)
		{
			return properties.Where(p => !StructureTypeReflecter.HasIdProperty(p.PropertyType)).ToArray();
		}
	}
}