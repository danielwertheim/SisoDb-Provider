using System;
using System.Data;
using System.Linq;
using NCore;
using NUnit.Framework;
using PineCone.Annotations;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Updates
{
    [TestFixture]
    public class Sql2008UnitOfWorkUpdateTests : Sql2008IntegrationTestBase
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

                var refetchedA = unitOfWork.GetById<SimpleItem>(item.StructureId);
                Assert.AreEqual(item.StructureId, refetchedA.StructureId);

                refetchedA.Value = "Come on!";
                unitOfWork.Update(refetchedA);
                unitOfWork.Commit();

                var refetchedB = unitOfWork.GetById<SimpleItem>(item.StructureId);
                Assert.AreEqual(item.StructureId, refetchedA.StructureId);
                Assert.AreEqual(item.StructureId, refetchedB.StructureId);
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

                var refetchedA = unitOfWork.GetById<SimpleItem>(item.StructureId);
                refetchedA.Value = "Come on!";

                unitOfWork.Update(refetchedA);
                unitOfWork.Commit();

                var refetchedB = unitOfWork.GetById<SimpleItem>(item.StructureId);
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
                Database.StructureSchemas.GetSchema(TypeFor<SimpleItem>.Type).IndexAccessors
                .Single(iac => iac.Path.StartsWith("Value")).Path;
            var table = DbHelper.GetTableBySql(
                "select [{0}] from dbo.SimpleItemIndexes where StructureId = '{1}'".Inject(propertyHash, item.StructureId));
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
                var refetched = unitOfWork.GetById<SimpleItem>(item.StructureId);
                refetched.Value = "B";

                unitOfWork.Update(refetched);
                unitOfWork.Commit();
            }

            var propertyHash =
                Database.StructureSchemas.GetSchema(TypeFor<SimpleItem>.Type).IndexAccessors
                .Single(iac => iac.Path.StartsWith("Value")).Path;
            var table = DbHelper.GetTableBySql(
                "select [{0}] from dbo.SimpleItemIndexes where StructureId = '{1}'".Inject(propertyHash, item.StructureId));
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
                var refetched = unitOfWork.GetById<TestItem>(item.StructureId);
                refetched.Key = "B";

                unitOfWork.Update(refetched);
                unitOfWork.Commit();
            }

            var table = DbHelper.GetTableBySql(
                "select UqValue from dbo.TestItemUniques where UqStructureId is null;");
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("B", table.AsEnumerable().First()["UqValue"]);
        }

        private class SimpleItem
        {
            public Guid StructureId { get; set; }

            public string Value { get; set; }
        }

        private class TestItem
        {
            public Guid StructureId { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }
    }
}