using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureIdPropertyNamesTests : UnitTestBase
    {
        [Test]
        public void Default_ReturnsStructureId()
        {
            Assert.AreEqual("StructureId", StructureIdPropertyNames.Default);
        }

        [Test]
        public void GetTypeNamePropertyNameFor_WhenClass_ReturnsModelId()
        {
            Assert.AreEqual("ModelId", StructureIdPropertyNames.GetTypeNamePropertyNameFor(typeof(Model)));
        }

        [Test]
        public void GetInterfaceTypeNamePropertyNameFor_WhenInterface_ReturnsModelId()
        {
            Assert.AreEqual("ModelId", StructureIdPropertyNames.GetInterfaceTypeNamePropertyNameFor(typeof(IModel)));
        }

        private interface IModel
        {
        }

        private class Model
        {
        }
    }
}