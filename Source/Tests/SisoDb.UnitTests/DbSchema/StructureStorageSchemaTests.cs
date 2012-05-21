using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class StructureStorageSchemaTests : StorageSchemaTestBase
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequenece()
        {
            var fields = StructureStorageSchema.OrderedFields;

            Assert.AreEqual(StructureStorageSchema.Fields.RowId, fields[0]);
            Assert.AreEqual(StructureStorageSchema.Fields.Id, fields[1]);
            Assert.AreEqual(StructureStorageSchema.Fields.Json, fields[2]);
        }

        [Test]
        public void SchemaField_RowId_HasCorrectName()
        {
            var field = StructureStorageSchema.Fields.RowId;

            Assert.AreEqual("RowId", field.Name);
        }

        [Test]
        public void SchemaField_RowId_HasCorrectOrdinal()
        {
            var field = StructureStorageSchema.Fields.RowId;

            Assert.AreEqual(0, field.Ordinal);
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

            Assert.AreEqual(1, field.Ordinal);
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

            Assert.AreEqual(2, field.Ordinal);
        }

        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var structureStorageSchema = new StructureStorageSchema(structureSchema, "foo");

            var fieldsByIndex = structureStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(3, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(StructureStorageSchema.Fields.RowId.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual(StructureStorageSchema.Fields.Id.Name, fieldsByIndex[1].Name);

            Assert.AreEqual(2, fieldsByIndex[2].Ordinal);
            Assert.AreEqual(StructureStorageSchema.Fields.Json.Name, fieldsByIndex[2].Name);
        }
    }
}