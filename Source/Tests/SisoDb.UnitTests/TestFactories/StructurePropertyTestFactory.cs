using System.Linq;
using PineCone.Structures.Schemas;

namespace SisoDb.UnitTests.TestFactories
{
    internal static class StructurePropertyTestFactory
    {
    	private static readonly StructureTypeReflecter Reflecter;

    	static StructurePropertyTestFactory()
    	{
    		Reflecter = new StructureTypeReflecter();
    	}

        internal static IStructureProperty GetIdProperty<T>()
        {
            return Reflecter.GetIdProperty(typeof(T));
        }

        internal static IStructureProperty GetPropertyByPath<T>(string path)
        {
            return Reflecter.GetIndexableProperties(typeof(T)).Single(i => i.Path == path);
        }

        internal static IStructureProperty GetPropertyByName<T>(string name)
        {
            return Reflecter.GetIndexableProperties(typeof(T)).Single(i => i.Name == name);
        }
    }
}