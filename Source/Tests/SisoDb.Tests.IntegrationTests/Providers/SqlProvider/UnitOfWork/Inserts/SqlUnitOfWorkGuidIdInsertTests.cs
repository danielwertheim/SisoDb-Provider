using System;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkGuidIdInsertTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForGuidIdInsertsWithPrivateSetter>();
        }

        [Test]
        public void Insert_WhenIdSetterIsPrivate_ItemIsInsertedAndIdIsAssigned()
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
                item = uow.GetById<ItemForGuidIdInsertsWithPrivateSetter>(id);
            }

            Assert.AreEqual(id, item.Id);
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
                item = uow.GetById<ItemForGuidIdInsertsWithPrivateSetter>(item.Id);
            }

            Assert.AreNotEqual(Guid.Empty, item.Id);
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
                item = uow.GetById<ItemForNullableGuidIdInsertsWithPrivateSetter>(item.Id.Value);
            }

            Assert.IsNotNull(item.Id);
            Assert.AreNotEqual(Guid.Empty, item.Id);
        }

        private class ItemForGuidIdInsertsWithPrivateSetter
        {
            public Guid Id { get; private set; }

            public string Temp
            {
                get { return "Some text to get rid of exception of no indexable members."; }
            }

            public ItemForGuidIdInsertsWithPrivateSetter()
            {

            }

            internal ItemForGuidIdInsertsWithPrivateSetter(Guid id)
            {
                Id = id;
            }
        }

        private class ItemForNullableGuidIdInsertsWithPrivateSetter
        {
            public Guid? Id { get; private set; }

            public string Temp
            {
                get { return "Some text to get rid of exception of no indexable members."; }
            }
        }
    }
}