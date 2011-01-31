using System;
using NUnit.Framework;
using SisoDb.Annotations;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Deletes
{
    [TestFixture]
    public class SqlUnitOfworkDeleteTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<SimpleGuidItem>();
            DropStructureSet<SimpleIdentityItem>();
        }

        [Test]
        public void DeleteByGuidId_WhenItemDoesNotExist_ReturnsFalse()
        {
            bool wasDeleted;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                wasDeleted = unitOfWork.DeleteById<SimpleGuidItem>(Guid.NewGuid());
                unitOfWork.Commit();
            }

            Assert.IsFalse(wasDeleted);
        }

        [Test]
        public void DeleteByIdentityId_WhenItemDoesNotExist_ReturnsFalse()
        {
            bool wasDeleted;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                wasDeleted = unitOfWork.DeleteById<SimpleIdentityItem>(1);
                unitOfWork.Commit();
            }

            Assert.IsFalse(wasDeleted);
        }

        [Test]
        public void DeleteByGuidId_WhenItemExists_ReturnsTrue()
        {
            bool wasDeleted;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                var item = new SimpleGuidItem { Key = "A" };
                unitOfWork.Insert(item);
                unitOfWork.Commit();

                wasDeleted = unitOfWork.DeleteById<SimpleGuidItem>(item.Id);
                unitOfWork.Commit();
            }

            Assert.IsTrue(wasDeleted);
        }

        [Test]
        public void DeleteByIdentityId_WhenItemExists_ReturnsTrue()
        {
            bool wasDeleted;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                var item = new SimpleIdentityItem { Key = "A" };
                unitOfWork.Insert(item);
                unitOfWork.Commit();

                wasDeleted = unitOfWork.DeleteById<SimpleIdentityItem>(item.Id);
                unitOfWork.Commit();
            }

            Assert.IsTrue(wasDeleted);
        }

        [Test]
        public void DeleteByGuidId_WhenItemExists_ItemIsDeletedFromAllTables()
        {
            var id = new Guid("35F3060A-DE6F-4BED-BFB9-D523EA98C49E");
            var item = new SimpleGuidItem { Id = id, Key = "A" };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();
            }

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.DeleteById<SimpleGuidItem>(id);
                unitOfWork.Commit();
            }

            var structureTable = GetStructureTableName<SimpleGuidItem>();
            var structureRowCount = DbHelper.RowCount(structureTable, "Id = '{0}'".Inject(id));
            Assert.AreEqual(0, structureRowCount);

            var indexesTable = GetIndexesTableName<SimpleGuidItem>();
            var indexesRowCount = DbHelper.RowCount(indexesTable, "StructureId = '{0}'".Inject(id));
            Assert.AreEqual(0, indexesRowCount);

            var uniquesTable = GetUniquesTableName<SimpleGuidItem>();
            var uniquesRowCount = DbHelper.RowCount(uniquesTable, "StructureId = '{0}'".Inject(id));
            Assert.AreEqual(0, uniquesRowCount);
        }

        [Test]
        public void DeleteByIdentityId_WhenItemExists_ItemIsDeletedFromAllTables()
        {
            var id = 1;
            var item = new SimpleIdentityItem { Id = 1, Key = "A" };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();
            }

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.DeleteById<SimpleIdentityItem>(id);
                unitOfWork.Commit();
            }

            var structureTable = GetStructureTableName<SimpleIdentityItem>();
            var structureRowCount = DbHelper.RowCount(structureTable, "Id = '{0}'".Inject(id));
            Assert.AreEqual(0, structureRowCount);

            var indexesTable = GetIndexesTableName<SimpleIdentityItem>();
            var indexesRowCount = DbHelper.RowCount(indexesTable, "StructureId = '{0}'".Inject(id));
            Assert.AreEqual(0, indexesRowCount);

            var uniquesTable = GetUniquesTableName<SimpleIdentityItem>();
            var uniquesRowCount = DbHelper.RowCount(uniquesTable, "StructureId = '{0}'".Inject(id));
            Assert.AreEqual(0, uniquesRowCount);
        }

        private class SimpleGuidItem
        {
            public Guid Id { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }

        private class SimpleIdentityItem
        {
            public int Id { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }
    }
}