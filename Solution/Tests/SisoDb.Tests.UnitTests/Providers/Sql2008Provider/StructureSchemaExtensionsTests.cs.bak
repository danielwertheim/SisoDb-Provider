using NUnit.Framework;
using SisoDb.Providers.SqlProvider;
using SisoDb.Structures.Schemas;
using TypeMock.ArrangeActAssert;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider
{
    [TestFixture]
    public class StructureSchemaExtensionsTests
    {
        [Test, Isolated]
        public void GetStructureTableName_ForStructure_ReturnsCorrectName()
        {
            var schema = Isolate.Fake.Instance<IStructureSchema>();
            Isolate.WhenCalled(() => schema.Name).WillReturn("Customer");
            Isolate.WhenCalled(() => schema.GetStructureTableName()).CallOriginal();

            var tableName = schema.GetStructureTableName();

            Assert.AreEqual("CustomerStructure", tableName);
        }

        [Test, Isolated]
        public void GetIndexesTableName_ForStructure_ReturnsCorrectName()
        {
            var schema = Isolate.Fake.Instance<IStructureSchema>();
            Isolate.WhenCalled(() => schema.Name).WillReturn("Customer");
            Isolate.WhenCalled(() => schema.GetStructureTableName()).CallOriginal();

            var tableName = schema.GetIndexesTableName();

            Assert.AreEqual("CustomerIndexes", tableName);
        }

        [Test, Isolated]
        public void GetUniquesTableName_ForStructure_ReturnsCorrectName()
        {
            var schema = Isolate.Fake.Instance<IStructureSchema>();
            Isolate.WhenCalled(() => schema.Name).WillReturn("Customer");
            Isolate.WhenCalled(() => schema.GetStructureTableName()).CallOriginal();

            var tableName = schema.GetUniquesTableName();

            Assert.AreEqual("CustomerUniques", tableName);
        }
    }
}