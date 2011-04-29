using NUnit.Framework;
using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class StructureStorageSchemaTests : UnitTestBase
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequenece()
        {
            var fields = StructureStorageSchema.Fields.OrderedFields;

            Assert.AreEqual(StructureStorageSchema.Fields.Id, fields[0]);
            Assert.AreEqual(StructureStorageSchema.Fields.Json, fields[1]);
        }

        [Test]
        public void SchemaField_Id_HasCorrectName()
        {
            var field = StructureStorageSchema.Fields.Id;

            Assert.AreEqual("SisoId", field.Name);
        }

        [Test]
        public void SchemaField_Id_HasCorrectOrdinal()
        {
            var field = StructureStorageSchema.Fields.Id;

            Assert.AreEqual(0, field.Ordinal);
        }

        [Test]
        public void SchemaField_Json_HasCorrectName()
        {
            var field = StructureStorageSchema.Fields.Json;

            Assert.AreEqual("Json", field.Name);
        }

        [Test]
        public void SchemaField_Json_HasCorrectOrdinal()
        {
            var field = StructureStorageSchema.Fields.Json;

            Assert.AreEqual(1, field.Ordinal);
        }
    }
}