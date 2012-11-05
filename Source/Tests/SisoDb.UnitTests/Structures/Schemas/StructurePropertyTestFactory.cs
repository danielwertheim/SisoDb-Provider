using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas
{
    internal static class StructurePropertyTestFactory
    {
        private static readonly IStructurePropertyFactory PropertyFactory = new StructurePropertyFactory();

        internal static IStructureProperty GetIdProperty<T>() where T : class 
        {
            return ReflecterFor().GetIdProperty(typeof(T));
        }

        internal static IStructureProperty GetPropertyByPath<T>(string path) where T : class 
        {
            return ReflecterFor().GetIndexableProperties(typeof(T), false).Single(i => i.Path == path);
        }

        internal static IStructureProperty GetPropertyByName<T>(string name) where T : class 
        {
            return ReflecterFor().GetIndexableProperties(typeof(T), false).Single(i => i.Name == name);
        }

        internal static IStructureProperty GetRawProperty<T>(string name) where T : class 
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(name);

            return PropertyFactory.CreateRootPropertyFrom(propertyInfo);
        }

        internal static IStructureProperty GetRawProperty<T>(string name, IStructureProperty parent) where T : class 
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(name);

            return PropertyFactory.CreateChildPropertyFrom(parent, propertyInfo);
        }

        private static IStructureTypeReflecter ReflecterFor()
        {
            return new StructureTypeReflecter();
        }
    }
}