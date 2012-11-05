using System;
using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.TestFactories
{
    internal static class StructurePropertyTestFactory
    {
        internal static IStructureProperty GetIdProperty(Type structureType)
        {
            return ReflecterFor().GetIdProperty(structureType);
        }

        internal static IStructureProperty GetPropertyByPath(Type structureType, string path)
        {
            return ReflecterFor().GetIndexableProperties(structureType).Single(i => i.Path == path);
        }

        private static IStructureTypeReflecter ReflecterFor()
        {
            return new StructureTypeReflecter();
        }
    }
}