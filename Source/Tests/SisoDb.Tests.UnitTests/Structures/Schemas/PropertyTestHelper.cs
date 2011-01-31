using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    internal class PropertyTestHelper
    {
        internal static Property GetProperty<T>(string name)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(name);

            var property = new Property(propertyInfo);

            return property;
        }

        internal static Property GetProperty<T>(string name, Property parent, int level)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(name);

            var property = new Property(level, parent, propertyInfo);

            return property;
        }
    }
}