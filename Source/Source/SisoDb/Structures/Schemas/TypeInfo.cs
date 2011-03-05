using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.Reflections;

namespace SisoDb.Structures.Schemas
{
    public static class TypeInfo<T>
    {
        private static readonly TypeInfo State;

        public static string Name
        {
            get { return State.Name; }
        }

        static TypeInfo()
        {
            State = new TypeInfo(typeof(T));
        }

        public static IProperty GetIdProperty()
        {
            return State.GetIdProperty();
        }

        public static IEnumerable<IProperty> GetIndexableProperties(IEnumerable<string> nonIndexableNames = null)
        {
            return State.GetIndexableProperties(nonIndexableNames);
        }

        public static IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            return State.GetSimpleIndexablePropertyInfos(nonIndexableNames);
        }

        public static IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            return State.GetComplexIndexablePropertyInfos(nonIndexableNames);
        }

        public static IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            return State.GetEnumerableIndexablePropertyInfos(nonIndexableNames);
        }
    }

    public class TypeInfo
    {
        private readonly Type _type;

        public const BindingFlags IdPropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        public const BindingFlags PropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        public const string IdName = "Id";

        public string Name { get; private set; }

        public TypeInfo(Type type)
        {
            _type = type.AssertNotNull("type");
            Name = _type.Name;
        }

        public IProperty GetIdProperty()
        {
            return GetIdProperty(_type);
        }

        public IEnumerable<IProperty> GetIndexableProperties(IEnumerable<string> nonIndexableNames = null)
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

        public IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            var properties = _type.GetProperties(PropertyBindingFlags)
                .Where(p => p.PropertyType.IsSimpleType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }

        public IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            var properties = _type.GetProperties(PropertyBindingFlags)
                .Where(p => 
                    !p.PropertyType.IsSimpleType() && 
                    !p.PropertyType.IsEnumerableType() &&
                    GetIdProperty(p.PropertyType) == null);

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }

        public static bool HasIdProperty(Type type)
        {
            return GetIdProperty(type) != null;
        }

        private static IProperty GetIdProperty(Type type)
        {
            var propertyInfo = type.GetProperties(IdPropertyBindingFlags)
               .Where(p => p.Name.Equals(IdName))
               .SingleOrDefault();

            if (propertyInfo == null)
                return null;

            return new Property(propertyInfo);
        }

        public IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IEnumerable<string> nonIndexableNames = null)
        {
            var properties = _type.GetProperties(PropertyBindingFlags)
                .Where(p => !p.PropertyType.IsSimpleType() && p.PropertyType.IsEnumerableType() && !p.PropertyType.IsEnumerableBytesType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }
    }
}