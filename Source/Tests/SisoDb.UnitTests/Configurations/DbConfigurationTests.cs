using System;
using Moq;
using NUnit.Framework;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
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
        public void UseGuidStructureIdGeneratorResolvedBy_Should_make_ForInserts_to_return_builder_with_configured_id_generator()
        {
            var serializerFake = new Mock<ISisoDbSerializer>();
            var structureBuilders = new StructureBuilders(() => serializerFake.Object);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureBuilders).Returns(structureBuilders);

            dbFake.Object.Configure().UseGuidStructureIdGeneratorResolvedBy(() => new EmptyStructureIdGenerator());
            var builder = structureBuilders.ForInserts(StructureSchemaTestFactory.Stub<MyClassWithGuidId>(generateIdAccessor: true), Mock.Of<IIdentityStructureIdGenerator>());

            Assert.AreEqual(typeof(EmptyStructureIdGenerator), builder.StructureIdGenerator.GetType());
        }

        [Test]
        public void Serializer_Should_assign_new_serialzer_on_Db()
        {
            var dbFake = new Mock<ISisoDatabase>();
            var serializerFake = new Mock<ISisoDbSerializer>();
            var optionsFake = new Mock<SerializerOptions>();
            serializerFake.Setup(f => f.Options).Returns(optionsFake.Object);
            dbFake.Setup(f => f.Serializer).Returns(serializerFake.Object);

            dbFake.Object.Configure().Serializer(o => o.DateSerializationMode = DateSerializationModes.TimestampOffset);

            optionsFake.VerifySet(f => f.DateSerializationMode = DateSerializationModes.TimestampOffset, Times.Once());
        }

        [Test]
        public void UseCacheProviderResolvedBy_Should_assign_new_cache_provider_on_Db()
        {
            var cacheProviderFake = Mock.Of<ICacheProvider>();
            var dbFake = new Mock<ISisoDatabase>();

            dbFake.Object.Configure().UseCacheProviderResolvedBy(() => cacheProviderFake);

            dbFake.VerifySet(f => f.CacheProvider = cacheProviderFake, Times.Once());
        }

        [Test]
        public void ForProduction_Should_dissallow_upsert_of_schemas_and_synchronization()
        {
            var settingsFake = new Mock<IDbSettings>();
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settingsFake.Object);

            dbFake.Object.Configure().ForProduction();

            settingsFake.VerifySet(f => f.AllowUpsertsOfSchemas = false);
            settingsFake.VerifySet(f => f.SynchronizeSchemaChanges = false);
        }

        [Test]
        public void UseSerializerResolvedBy_Should_assign_new_serialzer_on_Db()
        {
            var serializerFake = Mock.Of<ISisoDbSerializer>();
            var dbFake = new Mock<ISisoDatabase>();

            dbFake.Object.Configure().UseSerializerResolvedBy(() => serializerFake);

            dbFake.VerifySet(f => f.Serializer = serializerFake, Times.Once());
        }

        private class MyClassWithGuidId
        {
            public Guid StructureId { get; set; }
        }
    }
}