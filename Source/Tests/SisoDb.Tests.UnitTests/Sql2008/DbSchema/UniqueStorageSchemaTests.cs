using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.Tests.UnitTests.Sql2008.DbSchema
{
    [TestFixture]
    public class UniqueStorageSchemaTests : StorageSchemaTests
    {
        [Test]
        public void OrderedFields_ShouldBeReturnedInCorrectSequence()
        {
            var fields = UniqueStorageSchema.OrderedFields;

            Assert.AreEqual(UniqueStorageSchema.Fields.StructureId, fields[0]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqStructureId, fields[1]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqName, fields[2]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqValue, fields[3]);
        }

        [Test]
        public void SchemaField_SisoId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.StructureId;

            Assert.AreEqual("SisoId", field.Name);
        }

        [Test]
        public void SchemaField_SisoId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.StructureId;

            Assert.AreEqual(0, field.Ordinal);
        }

        [Test]
        public void SchemaField_UqSisoId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.UqStructureId;

            Assert.AreEqual("UqSisoId", field.Name);
        }

        [Test]
        public void SchemaField_UqSisoId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.UqStructureId;

            Assert.AreEqual(1, field.Ordinal);
        }

        [Test]
        public void SchemaField_UqName_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.UqName;

            Assert.AreEqual("UqName", field.Name);
        }

        [Test]
        public void SchemaField_UqName_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.UqName;

            Assert.AreEqual(2, field.Ordinal);
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

            Assert.AreEqual(3, field.Ordinal);
        }

        [Test]
        public void GetFieldsAsDelimitedOrderedString_WhenPlainAndUniquesExistsInSchema_FieldsFromSchemaAreNotReturned()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var uniqueStorageSchema = new UniqueStorageSchema(structureSchema);

            CollectionAssert.AreEqual("[SisoId],[UqSisoId],[UqName],[UqValue]", uniqueStorageSchema.GetFieldsAsDelimitedOrderedString());
        }

        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var uniqueStorageSchema = new UniqueStorageSchema(structureSchema);

            var fieldsByIndex = uniqueStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(4, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.StructureId.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqStructureId.Name, fieldsByIndex[1].Name);

            Assert.AreEqual(2, fieldsByIndex[2].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqName.Name, fieldsByIndex[2].Name);

            Assert.AreEqual(3, fieldsByIndex[3].Ordinal);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqValue.Name, fieldsByIndex[3].Name);
        }
    }
}