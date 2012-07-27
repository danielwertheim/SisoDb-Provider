using System;
using System.Linq;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.PineCone.Structures.Schemas.Configuration;

namespace SisoDb.UnitTests.TestFactories
{
    internal static class StructurePropertyTestFactory
    {
        internal static IStructureProperty GetIdProperty(Type structureType)
        {
            return ReflecterFor(structureType).GetIdProperty();
        }

        internal static IStructureProperty GetPropertyByPath(Type structureType, string path)
        {
            return ReflecterFor(structureType).GetIndexableProperties().Single(i => i.Path == path);
        }

        private static IStructureTypeReflecter ReflecterFor(Type structureType)
        {
            return new StructureTypeReflecter(new StructureTypeConfig(structureType));
        }
    }
}