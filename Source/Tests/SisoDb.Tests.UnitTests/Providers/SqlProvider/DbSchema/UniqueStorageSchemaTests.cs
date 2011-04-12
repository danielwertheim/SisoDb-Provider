using NUnit.Framework;
using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class UniqueStorageSchemaTests : UnitTestBase
    {
        [Test]
        public void GetOrderedFields_ShouldBeReturnedInCorrectSequence()
        {
            var fields = UniqueStorageSchema.Fields.GetOrderedFields();

            Assert.AreEqual(UniqueStorageSchema.Fields.SisoId, fields[0]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqSisoId, fields[1]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqName, fields[2]);
            Assert.AreEqual(UniqueStorageSchema.Fields.UqValue, fields[3]);
        }

        [Test]
        public void SchemaField_SisoId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.SisoId;

            Assert.AreEqual("SisoId", field.Name);
        }

        [Test]
        public void SchemaField_SisoId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.SisoId;

            Assert.AreEqual(0, field.Ordinal);
        }

        [Test]
        public void SchemaField_UqSisoId_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.UqSisoId;

            Assert.AreEqual("UqSisoId", field.Name);
        }

        [Test]
        public void SchemaField_UqSisoId_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.UqSisoId;

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
    }
}