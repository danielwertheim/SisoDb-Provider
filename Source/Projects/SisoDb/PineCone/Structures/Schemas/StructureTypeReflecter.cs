using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SisoDb.EnsureThat;
using SisoDb.NCore.Collections;
using SisoDb.NCore.Reflections;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructureTypeReflecter : IStructureTypeReflecter
    {
        protected const string ConcurrencyTokenMemberName = "ConcurrencyToken";
        protected static readonly string[] NonIndexableSystemMembers = new string[0];

        public const BindingFlags IdPropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        public const BindingFlags PropertyBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        public IStructurePropertyFactory PropertyFactory { protected get; set; }

        public StructureTypeReflecter()
        {
            PropertyFactory = new StructurePropertyFactory();
        }

        public virtual bool HasIdProperty(Type structureType)
        {
            return GetIdProperty(structureType) != null;
        }

        public virtual bool HasConcurrencyTokenProperty(Type structureType)
        {
            return GetConcurrencyTokenProperty(structureType) != null;
        }

        public virtual bool HasTimeStampProperty(Type structureType)
        {
            return GetTimeStampProperty(structureType) != null;
        }

        public virtual IStructureProperty GetIdProperty(Type structureType)
        {
            var properties = structureType.GetProperties(IdPropertyBindingFlags).Where(p => p.Name.EndsWith(StructureIdPropertyNames.Indicator)).ToArray();

            var defaultProp = GetDefaultStructureIdProperty(properties);
            if (defaultProp != null)
                return PropertyFactory.CreateRootPropertyFrom(defaultProp);

            var typeNamedIdProp = GetTypeNamedStructureIdProperty(structureType, properties);
            if (typeNamedIdProp != null)
                return PropertyFactory.CreateRootPropertyFrom(typeNamedIdProp);

            var interfaceNamedIdProp = GetInterfaceNamedStructureIdProperty(structureType, properties);
            if (interfaceNamedIdProp != null)
                return PropertyFactory.CreateRootPropertyFrom(interfaceNamedIdProp);

            var idProp = properties.SingleOrDefault(p => p.Name.Equals(StructureIdPropertyNames.Indicator));
            if (idProp != null)
                return PropertyFactory.CreateRootPropertyFrom(idProp);

            return null;
        }

        protected virtual PropertyInfo GetDefaultStructureIdProperty(IEnumerable<PropertyInfo> properties)
        {
            return properties.SingleOrDefault(p => p.Name.Equals(StructureIdPropertyNames.Default));
        }

        protected virtual PropertyInfo GetTypeNamedStructureIdProperty(Type structureType, IEnumerable<PropertyInfo> properties)
        {
            var propertyName = StructureIdPropertyNames.GetTypeNamePropertyNameFor(structureType);

            return properties.SingleOrDefault(p => p.Name.Equals(propertyName));
        }

        protected virtual PropertyInfo GetInterfaceNamedStructureIdProperty(Type structureType, IEnumerable<PropertyInfo> properties)
        {
            if (!structureType.IsInterface)
                return null;

            var propertyName = StructureIdPropertyNames.GetInterfaceTypeNamePropertyNameFor(structureType);

            return properties.SingleOrDefault(p => p.Name.Equals(propertyName));
        }

        public virtual IStructureProperty GetTimeStampProperty(Type structureType)
        {
            var properties = structureType.GetProperties(PropertyBindingFlags).Where(p => p.Name.EndsWith(StructureTimeStampPropertyNames.Indicator)).ToArray();

            var defaultProp = GetDefaultStructureTimeStampProperty(properties);
            if (defaultProp != null)
                return PropertyFactory.CreateRootPropertyFrom(defaultProp);

            var typeNamedProp = GetTypeNamedStructureTimeStampProperty(structureType, properties);
            if (typeNamedProp != null)
                return PropertyFactory.CreateRootPropertyFrom(typeNamedProp);

            var interfaceNamedProp = GetInterfaceNamedStructureTimeStampProperty(structureType, properties);
            if (interfaceNamedProp != null)
                return PropertyFactory.CreateRootPropertyFrom(interfaceNamedProp);

            var prop = properties.SingleOrDefault(p => p.Name.Equals(StructureTimeStampPropertyNames.Indicator));
            if (prop != null)
                return PropertyFactory.CreateRootPropertyFrom(prop);

            return null;
        }

        protected virtual PropertyInfo GetDefaultStructureTimeStampProperty(IEnumerable<PropertyInfo> properties)
        {
            return properties.SingleOrDefault(p => p.Name.Equals(StructureTimeStampPropertyNames.Default));
        }

        protected virtual PropertyInfo GetTypeNamedStructureTimeStampProperty(Type structureType, IEnumerable<PropertyInfo> properties)
        {
            var propertyName = StructureTimeStampPropertyNames.GetTypeNamePropertyNameFor(structureType);

            return properties.SingleOrDefault(p => p.Name.Equals(propertyName));
        }

        protected virtual PropertyInfo GetInterfaceNamedStructureTimeStampProperty(Type structureType, IEnumerable<PropertyInfo> properties)
        {
            if (!structureType.IsInterface)
                return null;

            var propertyName = StructureTimeStampPropertyNames.GetInterfaceTypeNamePropertyNameFor(structureType);

            return properties.SingleOrDefault(p => p.Name.Equals(propertyName));
        }

        public virtual IStructureProperty GetConcurrencyTokenProperty(Type structureType)
        {
            var propertyInfo = structureType.GetProperty(ConcurrencyTokenMemberName, PropertyBindingFlags);

            return propertyInfo == null
                ? null
                : PropertyFactory.CreateRootPropertyFrom(propertyInfo);
        }

        public virtual IStructureProperty[] GetIndexableProperties(Type structureType, bool includeContainedStructureMembers)
        {
            return GetIndexableProperties(structureType, null, includeContainedStructureMembers, NonIndexableSystemMembers, null);
        }

        public virtual IStructureProperty[] GetIndexablePropertiesExcept(Type structureType, bool includeContainedStructureMembers, ICollection<string> nonIndexablePaths)
        {
            Ensure.That(nonIndexablePaths, "nonIndexablePaths").HasItems();

            return GetIndexableProperties(structureType, null, includeContainedStructureMembers, NonIndexableSystemMembers.MergeWith(nonIndexablePaths).ToArray(), null);
        }

        public virtual IStructureProperty[] GetSpecificIndexableProperties(Type structureType, bool includeContainedStructureMembers, ICollection<string> indexablePaths)
        {
            Ensure.That(indexablePaths, "indexablePaths").HasItems();

            return GetIndexableProperties(structureType, null, includeContainedStructureMembers, NonIndexableSystemMembers, indexablePaths);
        }

        public virtual IStructureProperty[] GetContainedStructureProperties(Type structureType)
        {
            var propertyInfos = GetIndexablePropertyInfos(structureType, true);
            var complexPropertyInfos = GetComplexIndexablePropertyInfos(propertyInfos);

            return complexPropertyInfos
                .Where(p => GetIdProperty(p.PropertyType) != null)
                .Select(p => PropertyFactory.CreateChildPropertyFrom(null, p)).ToArray();
        }

        protected virtual IStructureProperty[] GetIndexableProperties(
            IReflect type,
            IStructureProperty parent,
            bool includeContainedStructureMembers,
            ICollection<string> nonIndexablePaths,
            ICollection<string> indexablePaths)
        {
            var initialPropertyInfos = GetIndexablePropertyInfos(type, includeContainedStructureMembers);
            if (initialPropertyInfos.Length == 0)
                return new IStructureProperty[] { };

            var properties = new List<IStructureProperty>();

            var simplePropertyInfos = GetSimpleIndexablePropertyInfos(initialPropertyInfos, parent, nonIndexablePaths, indexablePaths);
            properties.AddRange(simplePropertyInfos.Select(spi => PropertyFactory.CreateChildPropertyFrom(parent, spi)));

            initialPropertyInfos = initialPropertyInfos.Where(p => !simplePropertyInfos.Contains(p)).ToArray();

            foreach (var complexPropertyInfo in GetComplexIndexablePropertyInfos(initialPropertyInfos, parent, nonIndexablePaths, indexablePaths))
            {
                var complexProperty = PropertyFactory.CreateChildPropertyFrom(parent, complexPropertyInfo);
                var simpleComplexProps = GetIndexableProperties(
                    complexProperty.DataType, complexProperty, includeContainedStructureMembers, nonIndexablePaths, indexablePaths);

                var beforeCount = properties.Count;
                properties.AddRange(simpleComplexProps);

                if (properties.Count == beforeCount && complexProperty.DataType.IsValueType)
                    properties.Add(complexProperty);
            }

            foreach (var enumerablePropertyInfo in GetEnumerableIndexablePropertyInfos(initialPropertyInfos, parent, nonIndexablePaths, indexablePaths))
            {
                var enumerableProperty = PropertyFactory.CreateChildPropertyFrom(parent, enumerablePropertyInfo);
                if (enumerableProperty.ElementDataType.IsSimpleType())
                {
                    properties.Add(enumerableProperty);
                    continue;
                }

                var elementProperties = GetIndexableProperties(
                    enumerableProperty.ElementDataType,
                    enumerableProperty,
                    includeContainedStructureMembers,
                    nonIndexablePaths,
                    indexablePaths);

                properties.AddRange(elementProperties);
            }

            return properties.ToArray();
        }

        private PropertyInfo[] GetIndexablePropertyInfos(IReflect type, bool includeContainedStructureMembers)
        {
            return includeContainedStructureMembers
                ? type.GetProperties(PropertyBindingFlags)
                : type.GetProperties(PropertyBindingFlags).Where(p => HasIdProperty(p.PropertyType) == false).ToArray();
        }

        protected virtual PropertyInfo[] GetSimpleIndexablePropertyInfos(PropertyInfo[] properties, IStructureProperty parent = null, ICollection<string> nonIndexablePaths = null, ICollection<string> indexablePaths = null)
        {
            if (properties.Length == 0)
                return new PropertyInfo[0];

            var filteredProperties = properties.Where(p => p.PropertyType.IsSimpleType());

            if (nonIndexablePaths != null && nonIndexablePaths.Any())
                filteredProperties = filteredProperties.Where(p => !nonIndexablePaths.Contains(
                    PropertyPathBuilder.BuildPath(parent, p.Name)));

            if (indexablePaths != null && indexablePaths.Any())
                filteredProperties = filteredProperties.Where(p => indexablePaths.Contains(
                    PropertyPathBuilder.BuildPath(parent, p.Name)));

            return filteredProperties.ToArray();
        }

        protected virtual PropertyInfo[] GetComplexIndexablePropertyInfos(PropertyInfo[] properties, IStructureProperty parent = null, ICollection<string> nonIndexablePaths = null, ICollection<string> indexablePaths = null)
        {
            if (properties.Length == 0)
                return new PropertyInfo[0];

            var filteredProperties = properties.Where(p =>
                !p.PropertyType.IsSimpleType() &&
                !p.PropertyType.IsEnumerableType());

            //if (!includeContainedStructureMembers)
            //    filteredProperties = filteredProperties.Where(p => GetIdProperty(p.PropertyType) == null);

            if (nonIndexablePaths != null && nonIndexablePaths.Any())
                filteredProperties = filteredProperties.Where(p => !nonIndexablePaths.Contains(
                    PropertyPathBuilder.BuildPath(parent, p.Name)));

            if (indexablePaths != null && indexablePaths.Any())
                filteredProperties = filteredProperties.Where(p => indexablePaths.Contains(
                    PropertyPathBuilder.BuildPath(parent, p.Name)));

            return filteredProperties.ToArray();
        }

        protected virtual PropertyInfo[] GetEnumerableIndexablePropertyInfos(PropertyInfo[] properties, IStructureProperty parent = null, ICollection<string> nonIndexablePaths = null, ICollection<string> indexablePaths = null)
        {
            if (properties.Length == 0)
                return new PropertyInfo[0];

            var filteredProperties = properties.Where(p =>
                !p.PropertyType.IsSimpleType() &&
                p.PropertyType.IsEnumerableType() &&
                !p.PropertyType.IsEnumerableBytesType());

            if (nonIndexablePaths != null && nonIndexablePaths.Any())
                filteredProperties = filteredProperties.Where(p => !nonIndexablePaths.Contains(
                    PropertyPathBuilder.BuildPath(parent, p.Name)));

            if (indexablePaths != null && indexablePaths.Any())
                filteredProperties = filteredProperties.Where(p => indexablePaths.Contains(
                    PropertyPathBuilder.BuildPath(parent, p.Name)));

            return filteredProperties.ToArray();
        }
    }
}