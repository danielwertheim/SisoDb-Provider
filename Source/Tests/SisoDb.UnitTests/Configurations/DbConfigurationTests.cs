using System;
using Moq;
using NUnit.Framework;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
using SisoDb.Configurations;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Configurations
{
    [TestFixture]
    public class DbConfigurationTests : UnitTestBase
    {
        [Test]
        public void UseManualStructureIdAssignment_Should_make_ForInserts_to_return_builder_with_EmptyStructureIdGenerator()
        {
            var serializerFake = new Mock<ISisoDbSerializer>();
            var structureBuilders = new StructureBuilders(() => serializerFake.Object);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureBuilders).Returns(structureBuilders);

            dbFake.Object.Configure().UseManualStructureIdAssignment();
            var builder = structureBuilders.ForInserts(Mock.Of<IStructureSchema>(), Mock.Of<IIdentityStructureIdGenerator>());

            Assert.AreEqual(typeof(EmptyStructureIdGenerator), builder.StructureIdGenerator.GetType());
        }

        [Test]
        public void ResolveGuidStructureIdGeneratorBy_Should_make_ForInserts_to_return_builder_with_configured_id_generator()
        {
            var serializerFake = new Mock<ISisoDbSerializer>();
            var structureBuilders = new StructureBuilders(() => serializerFake.Object);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureBuilders).Returns(structureBuilders);

            dbFake.Object.Configure().ResolveGuidStructureIdGeneratorBy(() => new EmptyStructureIdGenerator());
            var builder = structureBuilders.ForInserts(StructureSchemaTestFactory.Stub<MyClassWithGuidId>(generateIdAccessor: true), Mock.Of<IIdentityStructureIdGenerator>());

            Assert.AreEqual(typeof(EmptyStructureIdGenerator), builder.StructureIdGenerator.GetType());
        }

        [Test]
        public void UseSerializerOf_Should_assign_new_serialzer_on_Db()
        {
            var serializerFake = Mock.Of<ISisoDbSerializer>();
            var dbFake = new Mock<ISisoDatabase>();

            dbFake.Object.Configure().UseSerializerOf(serializerFake);

            dbFake.VerifySet(f => f.Serializer = serializerFake, Times.Once());
        }

        private class MyClassWithGuidId
        {
            public Guid StructureId { get; set; }
        }
    }
}