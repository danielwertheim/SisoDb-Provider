using System;
using Moq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;
using SisoDb.Structures.Schemas.MemberAccessors;
using SisoDb.UnitTests.TestFactories;
using System.Linq;

namespace SisoDb.UnitTests.Configurations
{
    [TestFixture]
    public class DbConfigurationTests : UnitTestBase
    {
        [Test]
        public void PreserveIds_Should_make_ResolveBuilderForInsert_to_return_builder_preservingIds_but_still_resolve_idgenerators()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.SetupGet(f => f.IdAccessor).Returns(() =>
            {
                var id = new Mock<IIdAccessor>();
                id.SetupGet(f => f.IdType).Returns(StructureIdTypes.Guid);
                return id.Object;
            });
            var serializerFake = new Mock<ISisoSerializer>();
            var structureBuilders = new StructureBuilders(
                () => serializerFake.Object,
                s => Mock.Of<IStructureIdGenerator>(),
                (s, d) => Mock.Of<IIdentityStructureIdGenerator>());
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureBuilders).Returns(structureBuilders);

            dbFake.Object.Configure().PreserveIds();
            var builder = structureBuilders.ResolveBuilderForInsert(schemaFake.Object, Mock.Of<IDbClient>());

            Assert.AreEqual(typeof(StructureBuilderPreservingId), builder.GetType());
        }

        [Test]
        public void UseManualStructureIdAssignment_Should_make_ResolveBuilderForInsert_to_return_builder_with_EmptyStructureIdGenerator()
        {
            var serializerFake = new Mock<ISisoSerializer>();
            var structureBuilders = new StructureBuilders(
                () => serializerFake.Object, 
                s => Mock.Of<IStructureIdGenerator>(), 
                (s, d) => Mock.Of<IIdentityStructureIdGenerator>());
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureBuilders).Returns(structureBuilders);

            dbFake.Object.Configure().UseManualStructureIdAssignment();
            var builder = structureBuilders.ResolveBuilderForInsert(Mock.Of<IStructureSchema>(), Mock.Of<IDbClient>());

            Assert.AreEqual(typeof(EmptyStructureIdGenerator), builder.StructureIdGenerator.GetType());
        }

        [Test]
        public void UseGuidStructureIdGeneratorResolvedBy_Should_make_ResolveBuilderForInsert_to_return_builder_with_configured_id_generator()
        {
            var serializerFake = new Mock<ISisoSerializer>();
            var structureBuilders = new StructureBuilders(
                () => serializerFake.Object,
                s => Mock.Of<IStructureIdGenerator>(),
                (s, d) => Mock.Of<IIdentityStructureIdGenerator>());
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureBuilders).Returns(structureBuilders);

            dbFake.Object.Configure().UseGuidStructureIdGeneratorResolvedBy(s => new EmptyStructureIdGenerator());
            var builder = structureBuilders.ResolveBuilderForInsert(StructureSchemaTestFactory.Stub<MyClassWithGuidId>(generateIdAccessor: true), Mock.Of<IDbClient>());

            Assert.AreEqual(typeof(EmptyStructureIdGenerator), builder.StructureIdGenerator.GetType());
        }

        [Test]
        public void UseCacheProvider_Should_assign_new_cache_provider_on_Db()
        {
            var cacheProviderFake = Mock.Of<ICacheProvider>();
            var dbFake = new Mock<ISisoDatabase>();

            dbFake.Object.Configure().UseCacheProvider(() => cacheProviderFake);

            dbFake.VerifySet(f => f.CacheProvider = cacheProviderFake, Times.Once());
        }

        [Test]
        public void ForProduction_Should_dissallow_upsert_of_schemas_and_synchronization()
        {
            var settingsFake = new Mock<IDbSettings>();
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settingsFake.Object);

            dbFake.Object.Configure().ForProduction();

            settingsFake.VerifySet(f => f.AllowDynamicSchemaCreation = false);
            settingsFake.VerifySet(f => f.AllowDynamicSchemaUpdates = false);
        }

        [Test]
        public void UseSerializer_Should_assign_new_serialzer_on_Db()
        {
            var serializerFake = Mock.Of<ISisoSerializer>();
            var dbFake = new Mock<ISisoDatabase>();

            dbFake.Object.Configure().UseSerializer(() => serializerFake);

            dbFake.VerifySet(f => f.Serializer = serializerFake, Times.Once());
        }

        [Test]
        public void StructureType_Should_forward_call_to_structure_type_configurations()
        {
            var structureType = typeof (StructureForConfigTests);
            var structureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureSchemas).Returns(structureSchemas);

            dbFake.Object.Configure().StructureType(structureType, cfg => cfg.DoNotIndexThis("IntValue"));

            var config = structureSchemas.StructureTypeFactory.Configurations.GetConfiguration(structureType);
            Assert.IsNotNull(config);
            Assert.AreEqual("IntValue", config.MemberPathsNotBeingIndexed.SingleOrDefault());
        }

        [Test]
        public void StructureType_using_generics_Should_forward_call_to_structure_type_configurations()
        {
            var structureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.StructureSchemas).Returns(structureSchemas);

            dbFake.Object.Configure().StructureType<StructureForConfigTests>(cfg => cfg.DoNotIndexThis(x => x.StringValue));

            var config = structureSchemas.StructureTypeFactory.Configurations.GetConfiguration<StructureForConfigTests>();
            Assert.IsNotNull(config);
            Assert.AreEqual("StringValue", config.MemberPathsNotBeingIndexed.SingleOrDefault());
        }

        private class MyClassWithGuidId
        {
            public Guid StructureId { get; set; }
        }

        private class StructureForConfigTests
        {
            public Guid StructureId { get; set; }
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }
    }
}