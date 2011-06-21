using System;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Inserts
{
    [TestFixture]
    public class Sql2008UnitOfWorkGuidIdInsertTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForGuidIdInsertsWithPrivateSetter>();
            DropStructureSet<ItemForNullableGuidIdInsertsWithPrivateSetter>();
        }

        [Test]
        public void Insert_WhenIdSetterIsPrivate_ItemIsInsertedAndIdIsNotAssignedButRegenerated()
        {
            var id = new Guid("174063DA-0315-4AB2-A527-C1450AFDE587");
            var item = new ItemForGuidIdInsertsWithPrivateSetter(id);

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            using(var uow = Database.CreateUnitOfWork())
            {
                item = uow.GetAll<ItemForGuidIdInsertsWithPrivateSetter>().Single();
            }

            Assert.AreNotEqual(id, item.SisoId);
        }

        [Test]
        public void Insert_WhenNoGuidIsAssignedToItem_GuidIsAutomaticallyAssigned()
        {
            var item = new ItemForGuidIdInsertsWithPrivateSetter();

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            using (var uow = Database.CreateUnitOfWork())
            {
                item = uow.GetById<ItemForGuidIdInsertsWithPrivateSetter>(item.SisoId);
            }

            Assert.AreNotEqual(Guid.Empty, item.SisoId);
        }

        [Test]
        public void Insert_WhenNoNullableGuidIsAssignedToItem_GuidIsAutomaticallyAssigned()
        {
            var item = new ItemForNullableGuidIdInsertsWithPrivateSetter();

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            using (var uow = Database.CreateUnitOfWork())
            {
                item = uow.GetById<ItemForNullableGuidIdInsertsWithPrivateSetter>(item.SisoId.Value);
            }

            Assert.IsNotNull(item.SisoId);
            Assert.AreNotEqual(Guid.Empty, item.SisoId);
        }

        private class ItemForGuidIdInsertsWithPrivateSetter
        {
            public Guid SisoId { get; private set; }

            public string Temp
            {
                get { return "Some text to get rid of exception of no indexable members."; }
            }

            public ItemForGuidIdInsertsWithPrivateSetter()
            {

            }

            internal ItemForGuidIdInsertsWithPrivateSetter(Guid id)
            {
                SisoId = id;
            }
        }

        private class ItemForNullableGuidIdInsertsWithPrivateSetter
        {
            public Guid? SisoId { get; private set; }

            public string Temp
            {
                get { return "Some text to get rid of exception of no indexable members."; }
            }
        }
    }
}