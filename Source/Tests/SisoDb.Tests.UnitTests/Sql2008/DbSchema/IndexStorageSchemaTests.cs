using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.Tests.UnitTests.Sql2008.DbSchema
{
    [TestFixture]
    public class IndexStorageSchemaTests : StorageSchemaTests
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequenece()
        {
            var fields = IndexStorageSchema.OrderedFields;

            Assert.AreEqual(IndexStorageSchema.Fields.StructureId, fields[0]);
            Assert.AreEqual(IndexStorageSchema.Fields.MemberPath, fields[1]);
            Assert.AreEqual(IndexStorageSchema.Fields.StringValue, fields[2]);
            Assert.AreEqual(IndexStorageSchema.Fields.IntegerValue, fields[3]);
            Assert.AreEqual(IndexStorageSchema.Fields.FractalValue, fields[4]);
            Assert.AreEqual(IndexStorageSchema.Fields.DateTimeValue, fields[5]);
            Assert.AreEqual(IndexStorageSchema.Fields.BoolValue, fields[6]);
            Assert.AreEqual(IndexStorageSchema.Fields.GuidValue, fields[7]);
        }

        [Test,
        TestCase(typeof(int)),
        TestCase(typeof(long)),
        TestCase(typeof(short)),
        TestCase(typeof(byte))]
        public void GetValueSchemaFieldForType_WhenIntegerNumericTypes_ReturnsIntegerValueSchemaField(Type type)
        {
            Assert.AreEqual(IndexStorageSchema.Fields.IntegerValue.Name,
                            IndexStorageSchema.GetValueSchemaFieldForType(type).Name);
        }

        [Test,
        TestCase(typeof(decimal)),
        TestCase(typeof(double)),
        TestCase(typeof(Single)),
        TestCase(typeof(float))]
        public void GetValueSchemaFieldForType_WhenFractalNumericTypes_ReturnsIntegerValueSchemaField(Type type)
        {
            Assert.AreEqual(IndexStorageSchema.Fields.FractalValue.Name,
                            IndexStorageSchema.GetValueSchemaFieldForType(type).Name);
        }

        [Test]
        public void GetValueSchemaFieldForType_WhenStringType_ReturnsStringValueSchemaField()
        {
            Assert.AreEqual(IndexStorageSchema.Fields.StringValue.Name,
                            IndexStorageSchema.GetValueSchemaFieldForType(typeof(string)).Name);
        }

        [Test]
        public void GetValueSchemaFieldForType_WhenGuidType_ReturnsStringValueSchemaField()
        {
            Assert.AreEqual(IndexStorageSchema.Fields.GuidValue.Name,
                            IndexStorageSchema.GetValueSchemaFieldForType(typeof(Guid)).Name);
        }

        [Test]
        public void GetValueSchemaFieldForType_WhenDateTimeType_ReturnsDateTimeValueSchemaField()
        {
            Assert.AreEqual(IndexStorageSchema.Fields.DateTimeValue.Name,
                            IndexStorageSchema.GetValueSchemaFieldForType(typeof(DateTime)).Name);
        }

        [Test]
        public void GetValueSchemaFieldForType_WhenBoolType_ReturnsBoolValueSchemaField()
        {
            Assert.AreEqual(IndexStorageSchema.Fields.BoolValue.Name,
                            IndexStorageSchema.GetValueSchemaFieldForType(typeof(bool)).Name);
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
        public void SchemaField_StringValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.StringValue;

            Assert.AreEqual("StringValue", field.Name);
        }

        [Test]
        public void SchemaField_StringValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.StringValue;

            Assert.AreEqual(2, field.Ordinal);
        }

        [Test]
        public void SchemaField_IntegerValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.IntegerValue;

            Assert.AreEqual("IntegerValue", field.Name);
        }

        [Test]
        public void SchemaField_IntegerValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.IntegerValue;

            Assert.AreEqual(3, field.Ordinal);
        }

        [Test]
        public void SchemaField_FractalValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.FractalValue;

            Assert.AreEqual("FractalValue", field.Name);
        }

        [Test]
        public void SchemaField_FractalValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.FractalValue;

            Assert.AreEqual(4, field.Ordinal);
        }

        [Test]
        public void SchemaField_DateTimeValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.DateTimeValue;

            Assert.AreEqual("DateTimeValue", field.Name);
        }

        [Test]
        public void SchemaField_DateTimeValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.DateTimeValue;

            Assert.AreEqual(5, field.Ordinal);
        }

        [Test]
        public void SchemaField_BoolValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.BoolValue;

            Assert.AreEqual("BoolValue", field.Name);
        }

        [Test]
        public void SchemaField_BoolValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.BoolValue;

            Assert.AreEqual(6, field.Ordinal);
        }

        [Test]
        public void SchemaField_GuidValue_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.GuidValue;

            Assert.AreEqual("GuidValue", field.Name);
        }

        [Test]
        public void SchemaField_GuidValue_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.GuidValue;

            Assert.AreEqual(7, field.Ordinal);
        }

        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var indexStorageSchema = new IndexStorageSchema(structureSchema);

            var fieldsByIndex = indexStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(8, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.StructureId.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.MemberPath.Name, fieldsByIndex[1].Name);

            Assert.AreEqual(2, fieldsByIndex[2].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.StringValue.Name, fieldsByIndex[2].Name);

            Assert.AreEqual(3, fieldsByIndex[3].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.IntegerValue.Name, fieldsByIndex[3].Name);

            Assert.AreEqual(4, fieldsByIndex[4].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.FractalValue.Name, fieldsByIndex[4].Name);

            Assert.AreEqual(5, fieldsByIndex[5].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.DateTimeValue.Name, fieldsByIndex[5].Name);

            Assert.AreEqual(6, fieldsByIndex[6].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.BoolValue.Name, fieldsByIndex[6].Name);

            Assert.AreEqual(7, fieldsByIndex[7].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.GuidValue.Name, fieldsByIndex[7].Name);
        }
    }
}