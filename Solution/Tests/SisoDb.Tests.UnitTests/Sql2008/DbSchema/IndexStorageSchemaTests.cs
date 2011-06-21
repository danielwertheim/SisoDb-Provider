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
            var fields = IndexStorageSchema.Fields.OrderedFields;

            Assert.AreEqual(IndexStorageSchema.Fields.SisoId, fields[0]);
        }

        [Test]
        public void SchemaField_SisoId_HasCorrectName()
        {
            var field = IndexStorageSchema.Fields.SisoId;

            Assert.AreEqual("SisoId", field.Name);
        }

        [Test]
        public void SchemaField_SisoId_HasCorrectOrdinal()
        {
            var field = IndexStorageSchema.Fields.SisoId;

            Assert.AreEqual(0, field.Ordinal);
        }

        [Test]
        public void GetFieldsAsDelimitedOrderedString_WhenPlainAndUniquesExistsInSchema_FixedFieldsPlusDynamicFieldsFromSchemaAreReturned()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var indexStorageSchema = new IndexStorageSchema(structureSchema);

            Assert.AreEqual("[SisoId],[Plain1],[Plain2],[Unique1],[Unique2]", indexStorageSchema.GetFieldsAsDelimitedOrderedString());
        }

        [Test]
        public void GetFieldsOrderedByIndex_WhenPlainAndUniquesExistsInSchema_FieldsAreReturnedInCorrectOrder()
        {
            var structureSchema = CreateStructureSchemaFakeWithPlainAndUniques();

            var indexStorageSchema = new IndexStorageSchema(structureSchema);

            var fieldsByIndex = indexStorageSchema.GetFieldsOrderedByIndex().ToList();

            Assert.AreEqual(5, fieldsByIndex.Count);

            Assert.AreEqual(0, fieldsByIndex[0].Ordinal);
            Assert.AreEqual(IndexStorageSchema.Fields.SisoId.Name, fieldsByIndex[0].Name);

            Assert.AreEqual(1, fieldsByIndex[1].Ordinal);
            Assert.AreEqual("Plain1", fieldsByIndex[1].Name);

            Assert.AreEqual(2, fieldsByIndex[2].Ordinal);
            Assert.AreEqual("Plain2", fieldsByIndex[2].Name);

            Assert.AreEqual(3, fieldsByIndex[3].Ordinal);
            Assert.AreEqual("Unique1", fieldsByIndex[3].Name);

            Assert.AreEqual(4, fieldsByIndex[4].Ordinal);
            Assert.AreEqual("Unique2", fieldsByIndex[4].Name);
        }
    }
}