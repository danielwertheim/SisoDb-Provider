using System;
using System.IO;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkInterfaceInsertTests : IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IMyItem>();
        }

        [Test]
        public void Insert_WhenInterfaceAsStructureSet_ItemStored()
        {
            var item = new MyItem { Name = "Daniel", Stream = new MemoryStream(BitConverter.GetBytes(333))};
            MyItem fetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<IMyItem>(item);
                uow.Commit();

                fetched = uow.GetByIdAs<IMyItem, MyItem>(item.Id);
            }

            Assert.AreEqual(item.Name, fetched.Name);
            Assert.AreNotEqual(item.Stream.Length, fetched.Stream.Length);
        }

        [Test]
        public void Insert_WhenInterfaceAsStructureSet_CanBeReadAsOtherImplementation()
        {
            var item = new MyItem { Name = "Daniel", Stream = new MemoryStream(BitConverter.GetBytes(333)) };
            MyItemInfo fetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<IMyItem>(item);
                uow.Commit();

                fetched = uow.GetByIdAs<IMyItem, MyItemInfo>(item.Id);
            }

            Assert.AreEqual(item.Name, fetched.Name);
        }

        public interface IMyItem
        {
            Guid Id { get; set; }

            string Name { get; }
        }

        public class MyItem : IMyItem
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public MemoryStream Stream { get; set; }

            public MyItem()
            {
                Stream = new MemoryStream();
            }
        }

        public class MyItemInfo : IMyItem
        {
            Guid IMyItem.Id { get; set; }

            public Guid Id { get; private set; }

            public string Name { get; private set; }
        }
    }
}