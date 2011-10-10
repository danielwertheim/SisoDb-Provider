using System;
using Moq;
using NUnit.Framework;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Tests.UnitTests.TestFactories;

namespace SisoDb.Tests.UnitTests.DbSchema
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
            var upserterFake = new Mock<IDbSchemaUpserter>();

            var manager = new DbSchemaManager();
            manager.UpsertStructureSet(_structureSchema, upserterFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema), Times.Once());
        }

        [Test]
        public void UpsertStructureSet_WhenCalledTwice_UpserterIsCalledOnceNotTwice()
        {
            var upserterFake = new Mock<IDbSchemaUpserter>();

            var manager = new DbSchemaManager();
            manager.UpsertStructureSet(_structureSchema, upserterFake.Object);
            manager.UpsertStructureSet(_structureSchema, upserterFake.Object);

            upserterFake.Verify(f => f.Upsert(_structureSchema), Times.Once());
        }

        [Test]
        public void DropStructureSet_WhenCalledTwice_DropperIsCalledTwice()
        {
            var dbClientFake = new Mock<IDbClient>();

            var manager = new DbSchemaManager();
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