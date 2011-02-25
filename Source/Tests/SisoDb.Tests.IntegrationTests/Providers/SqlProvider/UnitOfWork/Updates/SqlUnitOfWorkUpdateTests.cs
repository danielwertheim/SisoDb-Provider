using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;

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

                var refetchedA = unitOfWork.GetById<SimpleItem>(item.Id);
                Assert.AreEqual(item.Id, refetchedA.Id);

                refetchedA.Value = "Come on!";
                unitOfWork.Update(refetchedA);
                unitOfWork.Commit();

                var refetchedB = unitOfWork.GetById<SimpleItem>(item.Id);
                Assert.AreEqual(item.Id, refetchedA.Id);
                Assert.AreEqual(item.Id, refetchedB.Id);
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

                var refetchedA = unitOfWork.GetById<SimpleItem>(item.Id);
                refetchedA.Value = "Come on!";

                unitOfWork.Update(refetchedA);
                unitOfWork.Commit();

                var refetchedB = unitOfWork.GetById<SimpleItem>(item.Id);
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
                Database.StructureSchemas.GetSchema<SimpleItem>().IndexAccessors
                .Single(iac => iac.Name.StartsWith("Value")).Name;
            var table = DbHelper.GetTableBySql(
                "select [{0}] from dbo.SimpleItemIndexes where StructureId = '{1}'".Inject(propertyHash, item.Id));
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
                var refetched = unitOfWork.GetById<SimpleItem>(item.Id);
                refetched.Value = "B";

                unitOfWork.Update(refetched);
                unitOfWork.Commit();
            }

            var propertyHash =
                Database.StructureSchemas.GetSchema<SimpleItem>().IndexAccessors
                .Single(iac => iac.Name.StartsWith("Value")).Name;
            var table = DbHelper.GetTableBySql(
                "select [{0}] from dbo.SimpleItemIndexes where StructureId = '{1}'".Inject(propertyHash, item.Id));
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
                var refetched = unitOfWork.GetById<TestItem>(item.Id);
                refetched.Key = "B";

                unitOfWork.Update(refetched);
                unitOfWork.Commit();
            }

            var table = DbHelper.GetTableBySql(
                "select Value from dbo.TestItemUniques where StructureId = '{0}'".Inject(item.Id));
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("B", table.AsEnumerable().First()["Value"]);
        }

        private class SimpleItem
        {
            public Guid Id { get; set; }

            public string Value { get; set; }
        }

        private class TestItem
        {
            public Guid Id { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }
    }
}