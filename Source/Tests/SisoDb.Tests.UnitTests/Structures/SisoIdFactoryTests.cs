using System;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class SisoIdFactoryTests : UnitTestBase
    {
        [Test]
        public void GetId_WhenInt_ReturnsSisoIdForIdentity()
        {
            var schema = new AutoSchemaBuilder<IdentityItem>().CreateSchema();
            var item = new IdentityItem {SisoId = 32};

            var factory = new SisoIdFactory();
            var sisoId = factory.GetId(schema, item);

            Assert.AreEqual(32, sisoId.Value);
        }

        [Test]
        public void GetId_WhenNullableInt_ReturnsSisoIdForIdentity()
        {
            var schema = new AutoSchemaBuilder<NullableIdentityItem>().CreateSchema();
            var item = new NullableIdentityItem { SisoId = 32 };

            var factory = new SisoIdFactory();
            var sisoId = factory.GetId(schema, item);

            Assert.AreEqual(32, sisoId.Value);
        }

        [Test]
        public void GetId_WhenGuid_ReturnsSisoIdForIdentity()
        {
            var id = new Guid("A13D516B-C204-4544-BAFA-E94BB3408F98");
            var schema = new AutoSchemaBuilder<GuidItem>().CreateSchema();
            var item = new GuidItem { SisoId = id };

            var factory = new SisoIdFactory();
            var sisoId = factory.GetId(schema, item);

            Assert.AreEqual(id, sisoId.Value);
        }

        [Test]
        public void GetId_WhenNullableGuid_ReturnsSisoIdForIdentity()
        {
            var id = new Guid("1FD9C8AA-44F4-4A17-8A10-8C0880ACCF5A");
            var schema = new AutoSchemaBuilder<NullableGuidItem>().CreateSchema();
            var item = new NullableGuidItem { SisoId = id };

            var factory = new SisoIdFactory();
            var sisoId = factory.GetId(schema, item);

            Assert.AreEqual(id, sisoId.Value);
        }

        private class IdentityItem
        {
            public int SisoId { get; set; }

            public string Temp { get; set; }
        }

        private class NullableIdentityItem
        {
            public int? SisoId { get; set; }

            public string Temp { get; set; }
        }

        private class GuidItem
        {
            public Guid SisoId { get; set; }

            public string Temp { get; set; }
        }

        private class NullableGuidItem
        {
            public Guid? SisoId { get; set; }

            public string Temp { get; set; }
        }

        private class InvalidIdType
        {
            public decimal Id { get; set; }

            public string Temp { get; set; }
        }
    }
}