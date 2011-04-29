using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    internal static class StructurePropertyTestFactory
    {
        internal static IStructureProperty GetIdProperty<T>()
        {
            return SisoEnvironment.Resources.ResolveStructureTypeReflecter()
                .GetIdProperty(typeof (T));
        }

        internal static IStructureProperty GetPropertyByPath<T>(string path)
        {
            return SisoEnvironment.Resources.ResolveStructureTypeReflecter()
                .GetIndexableProperties(typeof(T))
                .Single(i => i.Path == path);
        }

        internal static IStructureProperty GetPropertyByName<T>(string name)
        {
            return SisoEnvironment.Resources.ResolveStructureTypeReflecter()
                .GetIndexableProperties(typeof(T))
                .Single(i => i.Name == name);
        }
    }
}