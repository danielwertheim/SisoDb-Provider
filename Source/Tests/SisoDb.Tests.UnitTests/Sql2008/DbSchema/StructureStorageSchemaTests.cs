using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.Tests.UnitTests.Sql2008.DbSchema
{
    [TestFixture]
    public class StructureStorageSchemaTests : StorageSchemaTests
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequenece()
        {
            var fields = StructureStorageSchema.OrderedFields;

            Assert.AreEqual(StructureStorageSchema.Fields.Id, fields[0]);
            Assert.AreEqual(StructureStorageSchema.Fields.Json, fields[1]);
        }

        [Test]
        public void SchemaField_Id_HasCorrectName()
        {
            var field = StructureStorageSchema.Fields.Id;

            Assert.AreEqual("StructureId", field.Name);
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

        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var structureStorageSchema = new StructureStorageSchema(structureSchema);

            var fieldsByIndex = structureStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(2, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(StructureStorageSchema.Fields.Id.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual(StructureStorageSchema.Fields.Json.Name, fieldsByIndex[1].Name);
        }
    }
}