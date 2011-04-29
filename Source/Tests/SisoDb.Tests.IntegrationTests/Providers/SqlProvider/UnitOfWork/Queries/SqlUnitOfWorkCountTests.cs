using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkCountTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForCountTests>();
        }

        [Test]
        public void Count_WhenNoItemsExists_ReturnsZero()
        {
            using(var uow = Database.CreateUnitOfWork())
            {
                Assert.AreEqual(0, uow.Count<ItemForCountTests>());
            }
        }

        [Test]
        public void Count_UsingExpressionWhenNoItemsExists_ReturnsZero()
        {
            using (var uow = Database.CreateUnitOfWork())
            {
                Assert.AreEqual(0, uow.Count<ItemForCountTests>(i => i.SortOrder == 0));
            }
        }

        [Test]
        public void Count_WhenTwoItemsExistsInUnCommittedUnitOfWork_ReturnsTwo()
        {
            var items = new[]
                        {
                            new ItemForCountTests{SortOrder = 1, Value = "A"},
                            new ItemForCountTests{SortOrder = 2, Value = "B"}
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);

                Assert.AreEqual(2, uow.Count<ItemForCountTests>());
            }
        }

        [Test]
        public void Count_WhenTwoItemsExists_ReturnsTwo()
        {
            var items = new[]
                        {
                            new ItemForCountTests{SortOrder = 1, Value = "A"},
                            new ItemForCountTests{SortOrder = 2, Value = "B"}
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                Assert.AreEqual(2, uow.Count<ItemForCountTests>());
            }
        }

        [Test]
        public void Count_WhenExpressionMatchesTwoOfThreeItems_ReturnsTwo()
        {
            var items = new[]
                        {
                            new ItemForCountTests{SortOrder = 1, Value = "A"},
                            new ItemForCountTests{SortOrder = 2, Value = "B"},
                            new ItemForCountTests{SortOrder = 3, Value = "C"}
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                Assert.AreEqual(2, uow.Count<ItemForCountTests>(i => i.SortOrder > 1));
            }
        }

        [Test]
        public void Count_WhenExpressionMatchesNoneOfThreeItems_ReturnsTwo()
        {
            var items = new[]
                        {
                            new ItemForCountTests{SortOrder = 1, Value = "A"},
                            new ItemForCountTests{SortOrder = 2, Value = "B"},
                            new ItemForCountTests{SortOrder = 3, Value = "C"}
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                Assert.AreEqual(0, uow.Count<ItemForCountTests>(i => i.SortOrder == 0));
            }
        }

        private class ItemForCountTests
        {
            public int SisoId { get; set; }

            public int SortOrder { get; set; }

            public string Value { get; set; }
        }
    }
}