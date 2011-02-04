using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SisoDb.Providers.SqlProvider;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkInterfaceInsertTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IMyItem>();
        }

        [Test]
        public void Insert_WhenImplementationHasMembersNotExistingInInterface_MemberIsNotStored()
        {
            var item = new MyItem { Name = "Daniel", Stream = new MemoryStream(BitConverter.GetBytes(333)) };
            MyItem fetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<IMyItem>(item);
                uow.Commit();

                fetched = uow.GetByIdAs<IMyItem, MyItem>(item.Id);
            }

            Assert.AreNotEqual(item.Stream.Length, fetched.Stream.Length);

            var indexesTableName = Database.StructureSchemas.GetSchema<IMyItem>().GetIndexesTableName();
            var columnName = SisoDbEnvironment.MemberNameGenerator.Generate("Stream");
            var hasColumnForStream = DbHelper.ColumnsExist(indexesTableName, columnName);
            Assert.IsFalse(hasColumnForStream);
        }

        [Test]
        public void Insert_WhenInterfaceAsStructureDefinition_CanBeReadBack()
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
        public void Insert_WhenInterfaceAsStructureDefinition_CanBeReadAsOtherImplementation()
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

        [Test]
        public void InsertMany_WhenInterfaceAsStructureDefinition_CanBeReadBack()
        {
            var items = new[]
                            {
                                new MyItem { Name = "Daniel1", Stream = new MemoryStream(BitConverter.GetBytes(333)) },
                                new MyItem { Name = "Daniel2", Stream = new MemoryStream(BitConverter.GetBytes(444)) },
                            };

            IList<MyItem> fetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany<IMyItem>(items);
                uow.Commit();

                fetched = uow.GetAllAs<IMyItem, MyItem>().ToList();
            }

            Assert.AreEqual(items[0].Name, fetched[0].Name);
            Assert.AreNotEqual(items[0].Stream.Length, fetched[0].Stream.Length);

            Assert.AreEqual(items[1].Name, fetched[1].Name);
            Assert.AreNotEqual(items[1].Stream.Length, fetched[1].Stream.Length);
        }

        [Test]
        public void InsertMany_WhenInterfaceAsStructureDefinition_CanBeReadAsOtherImplementation()
        {
            var items = new[]
                            {
                                new MyItem { Name = "Daniel1", Stream = new MemoryStream(BitConverter.GetBytes(333)) },
                                new MyItem { Name = "Daniel2", Stream = new MemoryStream(BitConverter.GetBytes(444)) },
                            };

            IList<MyItemInfo> fetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany<IMyItem>(items);
                uow.Commit();

                fetched = uow.GetAllAs<IMyItem, MyItemInfo>().ToList();
            }

            Assert.AreEqual(items[0].Name, fetched[0].Name);
            Assert.AreEqual(items[1].Name, fetched[1].Name);
        }

        public interface IMyItem
        {
            int Id { get; set; }

            string Name { get; }
        }

        public class MyItem : IMyItem
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public MemoryStream Stream { get; set; }

            public MyItem()
            {
                Stream = new MemoryStream();
            }
        }

        public class MyItemInfo : IMyItem
        {
            int IMyItem.Id { get; set; }

            public int Id { get; private set; }

            public string Name { get; private set; }
        }
    }
}