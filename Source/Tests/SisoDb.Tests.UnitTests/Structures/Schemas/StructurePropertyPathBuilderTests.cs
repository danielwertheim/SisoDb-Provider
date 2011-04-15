using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructurePropertyPathBuilderTests : UnitTestBase
    {
        [Test]
        public void BuildPath_ForFirstLevelScalar_NoRootButPathWithNoDelimitor()
        {
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy1>("Int1");
            
            var path = PropertyPathBuilder.BuildPath(property);

            Assert.AreEqual("Int1", path);
        }

        [Test]
        public void BuildPath_ForSecondLevelScalar_NoRootButPathWithDelimitor()
        {
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy1>("Dummy2.Bool1");

            var path = PropertyPathBuilder.BuildPath(property);

            Assert.AreEqual("Dummy2.Bool1", path);
        }

        private class Dummy1
        {
            public int Int1 { get; set; }

            public Dummy2 Dummy2 { get; set; }
        }

        private class Dummy2
        {
            public bool Bool1 { get; set; }
        }
    }
}