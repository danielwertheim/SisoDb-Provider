using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class IndexStorageSchemaTests : StorageSchemaTestBase
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequenece()
        {
            var fields = IndexStorageSchema.OrderedFields;

            Assert.AreEqual(IndexStorageSchema.Fields.StructureId, fields[0]);
            Assert.AreEqual(IndexStorageSchema.Fields.MemberPath, fields[1]);
			Assert.AreEqual(IndexStorageSchema.Fields.Value, fields[2]);
			Assert.AreEqual(IndexStorageSchema.Fields.StringValue, fields[3]);
            Assert.AreEqual(IndexStorageSchema.Fields.RowId, fields[4]);
        }

        [Test]
        public void SchemaField_StructureId_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.StructureId;

            Assert.AreEqual("StructureId", field.Name);
        }

        [Test]
        public void SchemaField_StructureId_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.StructureId;

            Assert.AreEqual(0, field.Ordinal);
        }

        [Test]
        public void SchemaField_MemberPath_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.MemberPath;

            Assert.AreEqual("MemberPath", field.Name);
        }

        [Test]
        public void SchemaField_MemberPath_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.MemberPath;

            Assert.AreEqual(1, field.Ordinal);
        }

		[Test]
		public void SchemaField_Value_HasCorrectName()
		{
			var field = IndexStorageSchema.Fields.Value;

			Assert.AreEqual("Value", field.Name);
		}

		[Test]
		public void SchemaField_Value_HasCorrectOrdinal()
		{
			var field = IndexStorageSchema.Fields.Value;

			Assert.AreEqual(2, field.Ordinal);
		}

        [Test]
        public void SchemaField_StringValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.StringValue;

            Assert.AreEqual("StringValue", field.Name);
        }

        [Test]
        public void SchemaField_StringValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.StringValue;

            Assert.AreEqual(3, field.Ordinal);
        }

        [Test]
        public void SchemaField_RowId_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.RowId;

            Assert.AreEqual("RowId", field.Name);
        }

        [Test]
        public void SchemaField_RowId_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.RowId;

            Assert.AreEqual(4, field.Ordinal);
        }
        
        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var indexStorageSchema = new IndexStorageSchema(structureSchema, "foo");

            var fieldsByIndex = indexStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(5, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.StructureId.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.MemberPath.Name, fieldsByIndex[1].Name);

			Assert.AreEqual(2, fieldsByIndex[2].Ordinal);
			Assert.AreEqual(IndexStorageSchema.Fields.Value.Name, fieldsByIndex[2].Name);

            Assert.AreEqual(3, fieldsByIndex[3].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.StringValue.Name, fieldsByIndex[3].Name);

            Assert.AreEqual(4, fieldsByIndex[4].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.RowId.Name, fieldsByIndex[4].Name);
		}
    }
}