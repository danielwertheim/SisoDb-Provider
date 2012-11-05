using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public abstract class StructureTypeReflecterTestsBase : UnitTestBase
    {
        protected IStructureTypeReflecter ReflecterFor()
        {
            return new StructureTypeReflecter();
        }
    }
}