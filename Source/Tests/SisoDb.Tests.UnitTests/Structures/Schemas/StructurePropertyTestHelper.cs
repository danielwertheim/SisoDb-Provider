using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    public class StructurePropertyTestHelper
    {
        internal static StructureProperty GetProperty<T>(string name)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(name);

            var property = new StructureProperty(propertyInfo);

            return property;
        }

        internal static StructureProperty GetProperty<T>(string name, StructureProperty parent)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(name);

            var property = new StructureProperty(parent, propertyInfo);

            return property;
        }
    }
}