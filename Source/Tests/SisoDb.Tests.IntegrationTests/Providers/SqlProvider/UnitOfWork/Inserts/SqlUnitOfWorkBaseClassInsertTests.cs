using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SisoDb.Providers.SqlProvider;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkBaseClassInsertTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<MyItemBase>();
        }

        [Test]
        public void Insert_WhenImplementationHasMembersNotExistingInBaseClass_MemberIsNotStored()
        {
            var item = new MyItem { Name = "Daniel", Stream = new MemoryStream(BitConverter.GetBytes(333)) };
            MyItem fetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<MyItemBase>(item);
                uow.Commit();

                fetched = uow.GetByIdAs<MyItemBase, MyItem>(item.SisoId);
            }

            Assert.AreNotEqual(item.Stream.Length, fetched.Stream.Length);

            var indexesTableName = Database.StructureSchemas.GetSchema(StructureTypeFor<MyItemBase>.Instance).GetIndexesTableName();
            var columnName = SisoDbEnvironment.ResourceContainer.ResolveMemberNameGenerator().Generate("Stream");
            var hasColumnForStream = DbHelper.ColumnsExist(indexesTableName, columnName);
            Assert.IsFalse(hasColumnForStream);
        }

        [Test]
        public void Insert_WhenBaseClassAsStructureDefinition_CanBeReadBack()
        {
            var item = new MyItem { Name = "Daniel", Stream = new MemoryStream(BitConverter.GetBytes(333)) };
            MyItem fetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<MyItemBase>(item);
                uow.Commit();

                fetched = uow.GetByIdAs<MyItemBase, MyItem>(item.SisoId);
            }

            Assert.AreEqual(item.Name, fetched.Name);
            Assert.AreNotEqual(item.Stream.Length, fetched.Stream.Length);
        }

        [Test]
        public void InsertMany_WhenBaseClassAsStructureDefinition_CanBeReadBack()
        {
            var items = new[]
                            {
                                new MyItem { Name = "Daniel1", Stream = new MemoryStream(BitConverter.GetBytes(333)) },
                                new MyItem { Name = "Daniel2", Stream = new MemoryStream(BitConverter.GetBytes(444)) },
                            };

            IList<MyItem> fetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany<MyItemBase>(items);
                uow.Commit();

                fetched = uow.GetAllAs<MyItemBase, MyItem>().ToList();
            }

            Assert.AreEqual(items[0].Name, fetched[0].Name);
            Assert.AreNotEqual(items[0].Stream.Length, fetched[0].Stream.Length);

            Assert.AreEqual(items[1].Name, fetched[1].Name);
            Assert.AreNotEqual(items[1].Stream.Length, fetched[1].Stream.Length);
        }

        public abstract class MyItemBase
        {
            public int SisoId { get; set; }

            public string Name { get; set; }
        }

        public class MyItem : MyItemBase
        {
            public MemoryStream Stream { get; set; }

            public MyItem()
            {
                Stream = new MemoryStream();
            }
        }
    }
}