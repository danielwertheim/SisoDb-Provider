using System;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureIdFactoryTests : UnitTestBase
    {
        [Test]
        public void GetId_WhenInt_ReturnsStructureIdForIdentity()
        {
            var schema = new AutoSchemaBuilder<IdentityItem>().CreateSchema();
            var item = new IdentityItem {Id = 32};

            var factory = new StructureIdFactory();
            var structureId = factory.GetId(schema, item);

            Assert.AreEqual(32, structureId.Value);
        }

        [Test]
        public void GetId_WhenNullableInt_ReturnsStructureIdForIdentity()
        {
            var schema = new AutoSchemaBuilder<NullableIdentityItem>().CreateSchema();
            var item = new NullableIdentityItem { Id = 32 };

            var factory = new StructureIdFactory();
            var structureId = factory.GetId(schema, item);

            Assert.AreEqual(32, structureId.Value);
        }

        [Test]
        public void GetId_WhenGuid_ReturnsStructureIdForIdentity()
        {
            var id = new Guid("A13D516B-C204-4544-BAFA-E94BB3408F98");
            var schema = new AutoSchemaBuilder<GuidItem>().CreateSchema();
            var item = new GuidItem { Id = id};

            var factory = new StructureIdFactory();
            var structureId = factory.GetId(schema, item);

            Assert.AreEqual(id, structureId.Value);
        }

        [Test]
        public void GetId_WhenNullableGuid_ReturnsStructureIdForIdentity()
        {
            var id = new Guid("1FD9C8AA-44F4-4A17-8A10-8C0880ACCF5A");
            var schema = new AutoSchemaBuilder<NullableGuidItem>().CreateSchema();
            var item = new NullableGuidItem { Id = id };

            var factory = new StructureIdFactory();
            var structureId = factory.GetId(schema, item);

            Assert.AreEqual(id, structureId.Value);
        }

        private class IdentityItem
        {
            public int Id { get; set; }

            public string Temp { get; set; }
        }

        private class NullableIdentityItem
        {
            public int? Id { get; set; }

            public string Temp { get; set; }
        }

        private class GuidItem
        {
            public Guid Id { get; set; }

            public string Temp { get; set; }
        }

        private class NullableGuidItem
        {
            public Guid? Id { get; set; }

            public string Temp { get; set; }
        }

        private class InvalidIdType
        {
            public decimal Id { get; set; }

            public string Temp { get; set; }
        }
    }
}