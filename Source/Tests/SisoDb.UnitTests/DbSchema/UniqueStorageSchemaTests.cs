using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class UniqueStorageSchemaTests : StorageSchemaTestBase
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequence()
        {
            var fields = UniqueStorageSchema.OrderedFields;

            Assert.AreEqual(UniqueStorageSchema.Fields.RowId, fields[0]);
            Assert.AreEqual(UniqueStorageSchema.Fields.StructureId, fields[1]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqStructureId, fields[2]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqMemberPath, fields[3]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqValue, fields[4]);
        }

        [Test]
        public void SchemaField_RowId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.RowId;

            Assert.AreEqual("RowId", field.Name);
        }

        [Test]
        public void SchemaField_RowId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.RowId;

            Assert.AreEqual(0, field.Ordinal);
        }

        [Test]
        public void SchemaField_StructureId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.StructureId;

            Assert.AreEqual("StructureId", field.Name);
        }

        [Test]
        public void SchemaField_StructureId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.StructureId;

            Assert.AreEqual(1, field.Ordinal);
        }

        [Test]
        public void SchemaField_UqStructureId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.UqStructureId;

            Assert.AreEqual("UqStructureId", field.Name);
        }

        [Test]
        public void SchemaField_UqStructureId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.UqStructureId;

            Assert.AreEqual(2, field.Ordinal);
        }

        [Test]
        public void SchemaField_UqMemberPath_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.UqMemberPath;

            Assert.AreEqual("UqMemberPath", field.Name);
        }

        [Test]
        public void SchemaField_UqMemberPath_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.UqMemberPath;

            Assert.AreEqual(3, field.Ordinal);
        }

        [Test]
        public void SchemaField_UqValue_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.UqValue;

            Assert.AreEqual("UqValue", field.Name);
        }

        [Test]
        public void SchemaField_UqValue_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.UqValue;

            Assert.AreEqual(4, field.Ordinal);
        }

        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var uniqueStorageSchema = new UniqueStorageSchema(structureSchema, "foo");

            var fieldsByIndex = uniqueStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(5, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.RowId.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.StructureId.Name, fieldsByIndex[1].Name);

            Assert.AreEqual(2, fieldsByIndex[2].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqStructureId.Name, fieldsByIndex[2].Name);

            Assert.AreEqual(3, fieldsByIndex[3].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqMemberPath.Name, fieldsByIndex[3].Name);

            Assert.AreEqual(4, fieldsByIndex[4].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqValue.Name, fieldsByIndex[4].Name);
        }
    }
}