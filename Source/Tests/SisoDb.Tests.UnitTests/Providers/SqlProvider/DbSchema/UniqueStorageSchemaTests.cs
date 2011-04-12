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

            Assert.AreEqual(UniqueStorageSchema.Fields.SisoIdRef, fields[0]);
            Assert.AreEqual(UniqueStorageSchema.Fields.SisoId, fields[1]);
            Assert.AreEqual(UniqueStorageSchema.Fields.Name, fields[2]);
            Assert.AreEqual(UniqueStorageSchema.Fields.Value, fields[3]);
        }

        [Test]
        public void SchemaField_SisoIdRef_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.SisoIdRef;

            Assert.AreEqual("SisoIdRef", field.Name);
        }

        [Test]
        public void SchemaField_SisoIdRef_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.SisoIdRef;

            Assert.AreEqual(0, field.Ordinal);
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

            Assert.AreEqual(1, field.Ordinal);
        }

        [Test]
        public void SchemaField_Name_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.Name;

            Assert.AreEqual("Name", field.Name);
        }

        [Test]
        public void SchemaField_Name_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.Name;

            Assert.AreEqual(2, field.Ordinal);
        }

        [Test]
        public void SchemaField_Value_HasCorrectName()
        {
            var field = UniqueStorageSchema.Fields.Value;

            Assert.AreEqual("Value", field.Name);
        }

        [Test]
        public void SchemaField_Value_HasCorrectOrdinal()
        {
            var field = UniqueStorageSchema.Fields.Value;

            Assert.AreEqual(3, field.Ordinal);
        }
    }
}