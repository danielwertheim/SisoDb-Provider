using System;
using System.Linq;
using System.Reflection;
using PineCone.Annotations;

namespace PineCone.Structures.Schemas
{
    public class StructurePropertyFactory : IStructurePropertyFactory
    {
        private static readonly Type UniqueAttributeType = typeof(UniqueAttribute);

        public virtual IStructureProperty CreateRootPropertyFrom(PropertyInfo propertyInfo)
        {
            return new StructureProperty(
                ConvertInfo(propertyInfo),
                DynamicPropertyFactory.GetterFor(propertyInfo),
                DynamicPropertyFactory.SetterFor(propertyInfo));
        }

        public virtual IStructureProperty CreateChildPropertyFrom(IStructureProperty parent, PropertyInfo propertyInfo)
        {
            return new StructureProperty(
                ConvertInfo(propertyInfo, parent),
                DynamicPropertyFactory.GetterFor(propertyInfo),
                DynamicPropertyFactory.SetterFor(propertyInfo));
        }

        protected virtual StructurePropertyInfo ConvertInfo(PropertyInfo propertyInfo, IStructureProperty parent = null)
        {
            return new StructurePropertyInfo(
                propertyInfo.Name, 
                propertyInfo.PropertyType, 
                parent, 
                GetUniqueMode(propertyInfo));
        }

        protected virtual UniqueModes? GetUniqueMode(PropertyInfo propertyInfo)
        {
            var uniqueAttribute = (UniqueAttribute)propertyInfo.GetCustomAttributes(UniqueAttributeType, true).FirstOrDefault();

            UniqueModes? uniqueMode = null;
            if (uniqueAttribute != null)
                uniqueMode = uniqueAttribute.Mode;

            return uniqueMode;
        }
    }
}