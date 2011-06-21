using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.SqlCe4.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlCe4UnitOfWorkInsertTests : SqlCe4IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            //DropStructureSet<IdentityItem>();
        }

        [Test]
        public void Insert_WhenSimpleIdentityItem_ItemIsStored()
        {
            //var item = new IdentityItem {Value = "A"};

            //IdentityItem refethced;

            //using (var uow = Database.CreateUnitOfWork())
            //{
            //    uow.Insert(item);
            //    uow.Commit();

            //    refethced = uow.GetById<IdentityItem>(item.SisoId);
            //}

            //CustomAssert.AreValueEqual(item, refethced);
        }

        private class IdentityItem
        {
            public int SisoId { get; set; }

            public string Value { get; set; }
        }
    }
}