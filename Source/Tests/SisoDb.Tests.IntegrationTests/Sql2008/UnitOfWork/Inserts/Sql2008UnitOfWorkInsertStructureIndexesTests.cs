using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Inserts
{
    [TestFixture]
    public class Sql2008UnitOfWorkInsertStructureIndexesTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForInsertStructureIndexesTests>();
        }

        [Test]
        public void Insert_WhenManyDifferentStructures_IndexesAreStoredAgainstCorrectId()
        {
            var numOfItems = 10;
            var structures = CreateStructures(numOfItems);
            IList<ItemForInsertStructureIndexesTests> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(structures);
                uow.Commit();

                refetched = uow.GetAll<ItemForInsertStructureIndexesTests>().ToList();
            }

            Assert.AreEqual(numOfItems, refetched.Count);

            var valueMemberName = "Value";

            var table = DbHelper.GetTableBySql("select * from dbo.ItemForInsertStructureIndexesTestsIndexes;");
            for (var c = 0; c < numOfItems; c++)
            {
                var expectedId = c + 1;
                var row = table.Rows[c];
                var id = row["StructureId"];
                var value = row[valueMemberName];

                Assert.AreEqual(expectedId, id);
                Assert.AreEqual("StructureId-" + expectedId, value);
            }
        }

        private static IList<ItemForInsertStructureIndexesTests> CreateStructures(int numOfItems)
        {
            var items = new List<ItemForInsertStructureIndexesTests>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new ItemForInsertStructureIndexesTests
                          {
                              StructureId = numOfItems + 1,
                              Value = "StructureId-" + (c + 1)
                          });

            return items;
        }

        private class ItemForInsertStructureIndexesTests
        {
            public int StructureId { get; set; }

            public string Value { get; set; }
        }
    }
}