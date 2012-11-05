using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureTimeStampPropertyNamesTests : UnitTestBase
    {
        [Test]
        public void Default_ReturnsStructureId()
        {
            Assert.AreEqual("StructureTimeStamp", StructureTimeStampPropertyNames.Default);
        }

        [Test]
        public void GetTypeNamePropertyNameFor_WhenClass_ReturnsModelId()
        {
            Assert.AreEqual("ModelTimeStamp", StructureTimeStampPropertyNames.GetTypeNamePropertyNameFor(typeof(Model)));
        }

        [Test]
        public void GetInterfaceTypeNamePropertyNameFor_WhenInterface_ReturnsModelId()
        {
            Assert.AreEqual("ModelTimeStamp", StructureTimeStampPropertyNames.GetInterfaceTypeNamePropertyNameFor(typeof(IModel)));
        }

        private interface IModel
        {
        }

        private class Model
        {
        }
    }
}