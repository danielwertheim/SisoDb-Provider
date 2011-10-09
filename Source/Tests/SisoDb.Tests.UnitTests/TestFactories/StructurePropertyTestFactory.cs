using System.Linq;
using PineCone.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.TestFactories
{
    internal static class StructurePropertyTestFactory
    {
        internal static IStructureProperty GetIdProperty<T>()
        {
            return SisoEnvironment.Resources.ResolveStructureSchemas().StructureTypeFactory.Reflecter
                .GetIdProperty(typeof(T));
        }

        internal static IStructureProperty GetPropertyByPath<T>(string path)
        {
            return SisoEnvironment.Resources.ResolveStructureSchemas().StructureTypeFactory.Reflecter
                .GetIndexableProperties(typeof(T))
                .Single(i => i.Path == path);
        }

        internal static IStructureProperty GetPropertyByName<T>(string name)
        {
            return SisoEnvironment.Resources.ResolveStructureSchemas().StructureTypeFactory.Reflecter
                .GetIndexableProperties(typeof(T))
                .Single(i => i.Name == name);
        }
    }
}