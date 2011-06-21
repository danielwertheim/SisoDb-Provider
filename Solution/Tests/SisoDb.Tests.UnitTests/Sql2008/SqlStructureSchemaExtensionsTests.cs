using Moq;
using NUnit.Framework;
using SisoDb.Providers;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Sql2008
{
    [TestFixture]
    public class SqlStructureSchemaExtensionsTests : UnitTestBase
    {
        [Test]
        public void GetStructureTableName_ForStructure_ReturnsCorrectName()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(s => s.Name).Returns("Customer");

            var tableName = schemaFake.Object.GetStructureTableName();

            Assert.AreEqual("CustomerStructure", tableName);
        }

        [Test]
        public void GetIndexesTableName_ForStructure_ReturnsCorrectName()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(s => s.Name).Returns("Customer");

            var tableName = schemaFake.Object.GetIndexesTableName();

            Assert.AreEqual("CustomerIndexes", tableName);
        }

        [Test]
        public void GetUniquesTableName_ForStructure_ReturnsCorrectName()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(s => s.Name).Returns("Customer");

            var tableName = schemaFake.Object.GetUniquesTableName();

            Assert.AreEqual("CustomerUniques", tableName);
        }
    }
}