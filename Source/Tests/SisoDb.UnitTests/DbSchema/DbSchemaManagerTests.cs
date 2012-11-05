using System;
using Moq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class DbSchemaManagerTests : UnitTestBase
    {
        private IStructureSchema _structureSchema;

        protected override void OnFixtureInitialize()
        {
            _structureSchema = StructureSchemaTestFactory.Stub<Class_53966417_B25D_49E1_966B_58754110781C>(
                indexAccessorsPaths: new[] { "IndexableMember1", "IndexableMember2" });
        }

        [Test]
        public void UpsertStructureSet_WhenNeverCalled_UpserterIsCalledOnce()
        {
            var settings = new Mock<IDbSettings>();
            settings.SetupGet(f => f.AllowDynamicSchemaCreation).Returns(true);
            settings.SetupGet(f => f.AllowDynamicSchemaUpdates).Returns(true);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settings.Object);
            var upserterFake = new Mock<IDbSchemaUpserter>();
			var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager(dbFake.Object, upserterFake.Object);
            manager.UpsertStructureSet(_structureSchema, dbClientFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema, dbClientFake.Object, true, true), Times.Once());
        }

        [Test]
        public void UpsertStructureSet_WhenCalledTwice_UpserterIsCalledOnceNotTwice()
        {
            var settings = new Mock<IDbSettings>();
            settings.SetupGet(f => f.AllowDynamicSchemaCreation).Returns(true);
            settings.SetupGet(f => f.AllowDynamicSchemaUpdates).Returns(true);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settings.Object);
            var upserterFake = new Mock<IDbSchemaUpserter>();
			var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager(dbFake.Object, upserterFake.Object);
            manager.UpsertStructureSet(_structureSchema, dbClientFake.Object);
			manager.UpsertStructureSet(_structureSchema, dbClientFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema, dbClientFake.Object, true, true), Times.Once());
        }

        [Test]
        public void UpsertStructureSet_WhenDbSettingsDoesNotAllowAnyChanges_UpserterIsNeverCalled()
        {
            var settings = new Mock<IDbSettings>();
            settings.SetupGet(f => f.AllowDynamicSchemaCreation).Returns(false);
            settings.SetupGet(f => f.AllowDynamicSchemaUpdates).Returns(false);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settings.Object);
            var upserterFake = new Mock<IDbSchemaUpserter>();
            var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager(dbFake.Object, upserterFake.Object);
            manager.UpsertStructureSet(_structureSchema, dbClientFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema, dbClientFake.Object, false, false), Times.Never());
        }

        [Test]
        public void UpsertStructureSet_WhenDbSettingsAllowsCreationButNotUpdate_UpserterIsCalledOnce()
        {
            var settings = new Mock<IDbSettings>();
            settings.SetupGet(f => f.AllowDynamicSchemaCreation).Returns(true);
            settings.SetupGet(f => f.AllowDynamicSchemaUpdates).Returns(false);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settings.Object);
            var upserterFake = new Mock<IDbSchemaUpserter>();
            var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager(dbFake.Object, upserterFake.Object);
            manager.UpsertStructureSet(_structureSchema, dbClientFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema, dbClientFake.Object, true, false), Times.Once());
        }

        [Test]
        public void UpsertStructureSet_WhenDbSettingsAllowsUpdateButNotCreation_UpserterIsCalledOnce()
        {
            var settings = new Mock<IDbSettings>();
            settings.SetupGet(f => f.AllowDynamicSchemaCreation).Returns(false);
            settings.SetupGet(f => f.AllowDynamicSchemaUpdates).Returns(true);
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Settings).Returns(settings.Object);
            var upserterFake = new Mock<IDbSchemaUpserter>();
            var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager(dbFake.Object, upserterFake.Object);
            manager.UpsertStructureSet(_structureSchema, dbClientFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema, dbClientFake.Object, false, true), Times.Once());
        }

        [Test]
        public void DropStructureSet_WhenCalledTwice_DropperIsCalledTwice()
        {
			var upserterFake = new Mock<IDbSchemaUpserter>();
            var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager(Mock.Of<ISisoDatabase>(), upserterFake.Object);
            manager.DropStructureSet(_structureSchema, dbClientFake.Object);
            manager.DropStructureSet(_structureSchema, dbClientFake.Object);

            dbClientFake.Verify(f => f.Drop(_structureSchema), Times.Exactly(2));
        }

        private class Class_53966417_B25D_49E1_966B_58754110781C
        {
            public Guid StructureId { get; set; }

            public string IndexableMember1 { get; set; }

            public int IndexableMember2 { get; set; }
        }
    }
}