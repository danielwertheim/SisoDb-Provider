using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.Core;
using SisoDb.Reflections;

namespace SisoDb.Structures.Schemas
{
    public class StructureTypeReflecter : IStructureTypeReflecter
    {
        private static readonly string[] NonIndexableSystemMembers = new[] { StructureSchema.IdMemberName };

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

        public IEnumerable<IStructureProperty> GetIndexableProperties(IReflect type)
        {
            type.AssertNotNull("type");

            return GetIndexableProperties(type, null, NonIndexableSystemMembers, null);
        }

        public IEnumerable<IStructureProperty> GetIndexablePropertiesExcept(IReflect type, IEnumerable<string> nonIndexablePaths)
        {
            type.AssertNotNull("type");
            nonIndexablePaths.AssertHasItems("indexablePaths");

            return GetIndexableProperties(type, null, NonIndexableSystemMembers.Union(nonIndexablePaths), null);
        }

        public IEnumerable<IStructureProperty> GetSpecificIndexableProperties(IReflect type, IEnumerable<string> indexablePaths)
        {
            type.AssertNotNull("type");
            indexablePaths.AssertHasItems("indexablePaths");

            return GetIndexableProperties(type, null, NonIndexableSystemMembers, indexablePaths);
        }

        private IEnumerable<IStructureProperty> GetIndexableProperties(IReflect type, IStructureProperty parent, IEnumerable<string> nonIndexablePaths, IEnumerable<string> indexablePaths)
        {
            var properties = new List<IStructureProperty>();

            var simplePropertyInfos = GetSimpleIndexablePropertyInfos(type, nonIndexablePaths);
            var simpleProperties = simplePropertyInfos.Select(spi => new StructureProperty(parent, spi));
            properties.AddRange(simpleProperties);

            var complexPropertyInfos = GetComplexIndexablePropertyInfos(type, nonIndexablePaths);
            foreach (var complexPropertyInfo in complexPropertyInfos)
            {
                var complexProperty = new StructureProperty(parent, complexPropertyInfo);
                var simpleComplexProps = GetIndexableProperties(
                    complexProperty.PropertyType, complexProperty, nonIndexablePaths, indexablePaths);

                properties.AddRange(simpleComplexProps);
            }

            var enumerablePropertyInfos = GetEnumerableIndexablePropertyInfos(type, nonIndexablePaths);
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
                                                               nonIndexablePaths,
                                                               indexablePaths);
                properties.AddRange(elementProperties);
            }

            return properties;
        }

        internal IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexablePaths = null)
        {
            var properties = type.GetProperties(PropertyBindingFlags)
                .Where(p => p.PropertyType.IsSimpleType());

            if (nonIndexablePaths != null)
                properties = properties.Where(p => !nonIndexablePaths.Contains(p.Name));

            return properties.ToArray();
        }

        internal IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexablePaths = null)
        {
            var properties = type.GetProperties(PropertyBindingFlags)
                .Where(p =>
                       !p.PropertyType.IsSimpleType() &&
                       !p.PropertyType.IsEnumerableType() &&
                       GetIdProperty(p.PropertyType) == null);

            if (nonIndexablePaths != null)
                properties = properties.Where(p => !nonIndexablePaths.Contains(p.Name));

            return properties.ToArray();
        }

        internal IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexablePaths = null)
        {
            var properties = type.GetProperties(PropertyBindingFlags)
                .Where(p => !p.PropertyType.IsSimpleType() && p.PropertyType.IsEnumerableType() && !p.PropertyType.IsEnumerableBytesType());

            if (nonIndexablePaths != null)
                properties = properties.Where(p => !nonIndexablePaths.Contains(p.Name));

            return properties.ToArray();
        }
    }
}