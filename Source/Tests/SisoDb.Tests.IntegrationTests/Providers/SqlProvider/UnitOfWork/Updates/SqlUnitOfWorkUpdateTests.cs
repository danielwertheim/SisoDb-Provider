using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Core;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Updates
{
    [TestFixture]
    public class SqlUnitOfWorkUpdateTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<TestItem>();
            DropStructureSet<SimpleItem>();
        }

        [Test]
        public void Update_WhenItemExists_EnsureIdIsNotChanged()
        {
            var item = new SimpleItem {Value = "Test"};

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();

                var refetchedA = unitOfWork.GetById<SimpleItem>(item.SisoId);
                Assert.AreEqual(item.SisoId, refetchedA.SisoId);

                refetchedA.Value = "Come on!";
                unitOfWork.Update(refetchedA);
                unitOfWork.Commit();

                var refetchedB = unitOfWork.GetById<SimpleItem>(item.SisoId);
                Assert.AreEqual(item.SisoId, refetchedA.SisoId);
                Assert.AreEqual(item.SisoId, refetchedB.SisoId);
            }
        }

        [Test]
        public void Update_WhenItemExists_EnsureValueOnItemsAreCorrect()
        {
            var item = new SimpleItem { Value = "Test" };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();

                var refetchedA = unitOfWork.GetById<SimpleItem>(item.SisoId);
                refetchedA.Value = "Come on!";

                unitOfWork.Update(refetchedA);
                unitOfWork.Commit();

                var refetchedB = unitOfWork.GetById<SimpleItem>(item.SisoId);
                Assert.AreEqual("Test", item.Value);
                Assert.AreEqual("Come on!", refetchedA.Value);
                Assert.AreEqual("Come on!", refetchedB.Value);
            }
        }

        [Test]
        public void Update_WhenSameItem_ValueIsUpdated()
        {
            var item = new SimpleItem { Value = "A" };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();
            }

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                item.Value = "B";

                unitOfWork.Update(item);
                unitOfWork.Commit();
            }

            var propertyHash =
                Database.StructureSchemas.GetSchema(StructureType<SimpleItem>.Instance).IndexAccessors
                .Single(iac => iac.Name.StartsWith("Value")).Name;
            var table = DbHelper.GetTableBySql(
                "select [{0}] from dbo.SimpleItemIndexes where SisoId = '{1}'".Inject(propertyHash, item.SisoId));
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("B", table.AsEnumerable().First()[0]);
        }

        [Test]
        public void Update_WhenRefetchedItem_ValueIsUpdated()
        {
            var item = new SimpleItem { Value = "A" };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();
            }

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                var refetched = unitOfWork.GetById<SimpleItem>(item.SisoId);
                refetched.Value = "B";

                unitOfWork.Update(refetched);
                unitOfWork.Commit();
            }

            var propertyHash =
                Database.StructureSchemas.GetSchema(StructureType<SimpleItem>.Instance).IndexAccessors
                .Single(iac => iac.Name.StartsWith("Value")).Name;
            var table = DbHelper.GetTableBySql(
                "select [{0}] from dbo.SimpleItemIndexes where SisoId = '{1}'".Inject(propertyHash, item.SisoId));
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("B", table.AsEnumerable().First()[0]);
        }

        [Test]
        public void Update_WithUniqueConstraint_UniqueConstraintValueIsUpdated()
        {
            var item = new TestItem { Key = "A" };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();
            }

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                var refetched = unitOfWork.GetById<TestItem>(item.SisoId);
                refetched.Key = "B";

                unitOfWork.Update(refetched);
                unitOfWork.Commit();
            }

            var table = DbHelper.GetTableBySql(
                "select UqValue from dbo.TestItemUniques where UqSisoId is null;");
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("B", table.AsEnumerable().First()["UqValue"]);
        }

        private class SimpleItem
        {
            public Guid SisoId { get; set; }

            public string Value { get; set; }
        }

        private class TestItem
        {
            public Guid SisoId { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }
    }
}