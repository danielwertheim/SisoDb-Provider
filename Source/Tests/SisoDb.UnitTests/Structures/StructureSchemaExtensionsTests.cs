using Moq;
using NUnit.Framework;
using SisoDb.DbSchema;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.UnitTests.Structures
{
    [TestFixture]
    public class StructureSchemaExtensionsTests : UnitTestBase
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

            var tableNames = schemaFake.Object.GetIndexesTableNames();

            Assert.AreEqual("CustomerIntegers", tableNames.IntegersTableName);
			Assert.AreEqual("CustomerFractals", tableNames.FractalsTableName);
			Assert.AreEqual("CustomerBooleans", tableNames.BooleansTableName);
			Assert.AreEqual("CustomerDates", tableNames.DatesTableName);
			Assert.AreEqual("CustomerGuids", tableNames.GuidsTableName);
			Assert.AreEqual("CustomerStrings", tableNames.StringsTableName);
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