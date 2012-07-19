using System;
using System.Linq;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.UnitTests.TestFactories
{
    internal static class StructurePropertyTestFactory
    {
    	private static readonly StructureTypeReflecter Reflecter;

    	static StructurePropertyTestFactory()
    	{
    		Reflecter = new StructureTypeReflecter();
    	}

        internal static IStructureProperty GetIdProperty(Type structureType)
        {
            return Reflecter.GetIdProperty(structureType);
        }

        internal static IStructureProperty GetPropertyByPath(Type structureType, string path)
        {
            return Reflecter.GetIndexableProperties(structureType).Single(i => i.Path == path);
        }
    }
}