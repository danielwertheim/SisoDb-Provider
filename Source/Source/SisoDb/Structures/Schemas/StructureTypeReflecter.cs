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

        public IStructureProperty GetIdProperty(IReflect type)
        {
            var propertyInfo = type.GetProperties(IdPropertyBindingFlags)
                .Where(p => p.Name.Equals(StructureSchema.IdMemberName))
                .SingleOrDefault();

            if (propertyInfo == null)
                return null;

            return new StructureProperty(propertyInfo);
        }

        public IEnumerable<IStructureProperty> GetIndexableProperties(IReflect type, IEnumerable<string> nonIndexableNames = null)
        {
            return GetIndexableProperties(type, null, nonIndexableNames);
        }

        private IEnumerable<IStructureProperty> GetIndexableProperties(IReflect type, IStructureProperty parent, IEnumerable<string> nonIndexableNames = null)
        {
            var properties = new List<IStructureProperty>();

            var simplePropertyInfos = GetSimpleIndexablePropertyInfos(type, nonIndexableNames);
            var simpleProperties = simplePropertyInfos.Select(spi => new StructureProperty(parent, spi));
            properties.AddRange(simpleProperties);

            var complexPropertyInfos = GetComplexIndexablePropertyInfos(type, nonIndexableNames);
            foreach (var complexPropertyInfo in complexPropertyInfos)
            {
                var complexProperty = new StructureProperty(parent, complexPropertyInfo);
                var simpleComplexProps = GetIndexableProperties(
                    complexProperty.PropertyType, complexProperty, nonIndexableNames);
                properties.AddRange(simpleComplexProps);
            }

            var enumerablePropertyInfos = GetEnumerableIndexablePropertyInfos(type, nonIndexableNames);
            foreach (var enumerablePropertyInfo in enumerablePropertyInfos)
            {
                var enumerableProperty = new StructureProperty(parent, enumerablePropertyInfo);
                if (enumerableProperty.ElementType.IsSimpleType())
                {
                    properties.Add(enumerableProperty);
                    continue;
                }

                var elementProperties = GetIndexableProperties(enumerableProperty.ElementType,
                                                               enumerableProperty,
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