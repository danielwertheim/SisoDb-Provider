using System;
using System.Linq;
using NCore;
using NUnit.Framework;
using System.Collections.Generic;
using PineCone.Annotations;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Deletes
{
    [TestFixture]
    public class Sql2008UnitOfWorkDeleteTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<SimpleGuidItem>();
            DropStructureSet<SimpleIdentityItem>();
        }

        [Test]
        public void DeleteByGuidId_WhenItemDoesNotExist_DoesNotFail()
        {
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.DeleteById<SimpleGuidItem>(SequentialGuid.NewSqlCompatibleGuid());
                unitOfWork.Commit();
            }
        }

        [Test]
        public void DeleteByIdentityId_WhenItemDoesNotExist_DoesNotFail()
        {
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.DeleteById<SimpleIdentityItem>(1);
                unitOfWork.Commit();
            }
        }

        [Test]
        public void DeleteByGuidId_WhenItemExists_ItemIsDeletedFromAllTables()
        {
            var id = new Guid("35F3060A-DE6F-4BED-BFB9-D523EA98C49E");
            var item = new SimpleGuidItem { StructureId = id, Key = "A" };

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
            var structureRowCount = DbHelper.RowCount(structureTable, "StructureId = '{0}'".Inject(id));
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
            var item = new SimpleIdentityItem { StructureId = 1, Key = "A" };

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
            var structureRowCount = DbHelper.RowCount(structureTable, "StructureId = '{0}'".Inject(id));
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
            public Guid StructureId { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }

        private class SimpleIdentityItem
        {
            public int StructureId { get; set; }

            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }
    }
}