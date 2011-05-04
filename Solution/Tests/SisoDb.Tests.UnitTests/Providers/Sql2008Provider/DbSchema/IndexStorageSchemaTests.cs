using NUnit.Framework;
using SisoDb.Providers.Sql2008Provider.DbSchema;

namespace SisoDb.Tests.UnitTests.Providers.Sql2008Provider.DbSchema
{
    [TestFixture]
    public class IndexStorageSchemaTests : UnitTestBase
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
    }
}