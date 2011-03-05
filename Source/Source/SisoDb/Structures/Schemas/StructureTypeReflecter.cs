using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.Reflections;

namespace SisoDb.Structures.Schemas
{
    public class StructureTypeReflecter : IStructureTypeReflecter
    {
        public const BindingFlags IdPropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        public const BindingFlags PropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        public bool HasIdProperty(IReflect type)
        {
            return GetIdProperty(type) != null;
        }

        public IProperty GetIdProperty(IReflect type)
        {
            var propertyInfo = type.GetProperties(IdPropertyBindingFlags)
                .Where(p => p.Name.Equals(StructureSchema.IdMemberName))
                .SingleOrDefault();

            if (propertyInfo == null)
                return null;

            return new Property(propertyInfo);
        }

        public IEnumerable<IProperty> GetIndexableProperties(IReflect type, IEnumerable<string> nonIndexableNames = null)
        {
            return GetIndexableProperties(type, 0, null, nonIndexableNames);
        }

        private IEnumerable<IProperty> GetIndexableProperties(IReflect type, int level, IProperty parent, IEnumerable<string> nonIndexableNames = null)
        {
            var properties = new List<IProperty>();

            var simplePropertyInfos = GetSimpleIndexablePropertyInfos(type, nonIndexableNames);
            var simpleProperties = simplePropertyInfos.Select(spi => new Property(level, parent, spi));
            properties.AddRange(simpleProperties);

            var complexPropertyInfos = GetComplexIndexablePropertyInfos(type, nonIndexableNames);
            foreach (var complexPropertyInfo in complexPropertyInfos)
            {
                var complexProperty = new Property(level, parent, complexPropertyInfo);
                var simpleComplexProps = GetIndexableProperties(
                    complexProperty.PropertyType, complexProperty.Level + 1, complexProperty, nonIndexableNames);
                properties.AddRange(simpleComplexProps);
            }

            var enumerablePropertyInfos = GetEnumerableIndexablePropertyInfos(type, nonIndexableNames);
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

        public IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexableNames = null)
        {
            var properties = type.GetProperties(PropertyBindingFlags)
                .Where(p => p.PropertyType.IsSimpleType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }

        public IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexableNames = null)
        {
            var properties = type.GetProperties(PropertyBindingFlags)
                .Where(p => 
                       !p.PropertyType.IsSimpleType() && 
                       !p.PropertyType.IsEnumerableType() &&
                       GetIdProperty(p.PropertyType) == null);

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }

        public IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexableNames = null)
        {
            var properties = type.GetProperties(PropertyBindingFlags)
                .Where(p => !p.PropertyType.IsSimpleType() && p.PropertyType.IsEnumerableType() && !p.PropertyType.IsEnumerableBytesType());

            if (nonIndexableNames != null)
                properties = properties.Where(p => !nonIndexableNames.Contains(p.Name));

            return properties.ToArray();
        }
    }
}