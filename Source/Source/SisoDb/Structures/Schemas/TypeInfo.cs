using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SisoDb.Structures.Schemas
{
    internal static class TypeInfo<T>
    {
        private static readonly TypeInfo TypeInfoState;

        internal static string Name
        {
            get { return TypeInfoState.Name; }
        }

        static TypeInfo()
        {
            TypeInfoState = new TypeInfo(typeof(T));
        }

        internal static IProperty GetIdProperty(string name)
        {
            return TypeInfoState.GetIdProperty(name);
        }

        internal static IEnumerable<IProperty> GetIndexableProperties(IEnumerable<string> nonIndexableNames = null)
        {
            return TypeInfoState.GetIndexableProperties(nonIndexableNames);
        }

        internal static IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            return TypeInfoState.GetSimpleIndexablePropertyInfos(nonIndexableNames);
        }

        internal static IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            return TypeInfoState.GetComplexIndexablePropertyInfos(nonIndexableNames);
        }

        internal static IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            return TypeInfoState.GetEnumerableIndexablePropertyInfos(nonIndexableNames);
        }
    }

    internal class TypeInfo
    {
        private readonly Type _type;

        private const BindingFlags IdPropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        private const BindingFlags PropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        internal string Name { get; private set; }

        internal string FullName { get; private set; }

        internal TypeInfo(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            _type = type;

            var typeFullName = (_type.FullName ?? "")
                .Replace("+", ".")
                .Replace("-", ".");

            FullName = typeFullName;

            Name = _type.Name;
        }

        internal IProperty GetIdProperty(string name)
        {
            var propertyInfo = _type.GetProperties(IdPropertyBindingFlags)
                .Where(p => p.Name.Equals(name))
                .SingleOrDefault();

            if (propertyInfo == null)
                return null;

            return new Property(propertyInfo);
        }

        internal IEnumerable<IProperty> GetIndexableProperties(IEnumerable<string> nonIndexableNames = null)
        {
            var indexableProperties = GetIndexableProperties(_type, 0, null, nonIndexableNames);

            return indexableProperties;
        }

        private static IEnumerable<IProperty> GetIndexableProperties(Type type, int level, IProperty parent, IEnumerable<string> nonIndexableNames = null)
        {
            var properties = new List<IProperty>();
            var typeInfo = new TypeInfo(type);

            var simplePropertyInfos = typeInfo.GetSimpleIndexablePropertyInfos(nonIndexableNames);
            var simpleProperties = simplePropertyInfos.Select(spi => new Property(level, parent, spi));
            properties.AddRange(simpleProperties);

            var complexPropertyInfos = typeInfo.GetComplexIndexablePropertyInfos(nonIndexableNames);
            foreach (var complexPropertyInfo in complexPropertyInfos)
            {
                var complexProperty = new Property(level, parent, complexPropertyInfo);
                var simpleComplexProps = GetIndexableProperties(
                    complexProperty.PropertyType, complexProperty.Level + 1, complexProperty, nonIndexableNames);
                properties.AddRange(simpleComplexProps);
            }

            var enumerablePropertyInfos = typeInfo.GetEnumerableIndexablePropertyInfos(nonIndexableNames);
            foreach (var enumerablePropertyInfo in enumerablePropertyInfos)
            {
                var enumerableProperty = new Property(level, parent, enumerablePropertyInfo);
                if (enumerableProperty.ElementType.IsSimpleType())
                {
                    properties.Add(enumerableProperty);
                    continue;
                }

                var elementProperties = GetIndexableProperties(enumerableProperty.ElementType,
                                                               enumerableProperty.Level + 1, enumerableProperty,
                                                               nonIndexableNames);
                properties.AddRange(elementProperties);
            }

            return properties;
        }

        internal IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            var properties = _type.GetProperties(PropertyBindingFlags)
                .Where(p => p.PropertyType.IsSimpleType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }

        internal IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            var properties = _type.GetProperties(PropertyBindingFlags)
                .Where(p => !p.PropertyType.IsSimpleType() && !p.PropertyType.IsEnumerableType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }

        internal IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            var properties = _type.GetProperties(PropertyBindingFlags)
                .Where(p => !p.PropertyType.IsSimpleType() && p.PropertyType.IsEnumerableType() && !p.PropertyType.IsEnumerableBytesType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }
    }
}