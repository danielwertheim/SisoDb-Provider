using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Queries.QxExtensions
{
    [TestFixture]
    public class Sql2008UnitOfWorkQueryEnumerableWithQxAnyTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<QxItemForQueries>();
        }

        [Test]
        public void QueryStringArrayUsingQxAny_MatchingFirstElementInArray_ItemIsReturned()
        {
            var item = new QxItemForQueries { Strings = new[] { "Alpha", "Bravo", "Charlie" } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(q => 
                    q.Where(i => i.Strings.QxAny(e => e == "Alpha"))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.Strings, refetched.Strings);
        }

        [Test]
        public void QueryStringArrayUsingQxAny_SecondCriteriaMatchesMiddleElementInArray_ItemIsReturned()
        {
            var item = new QxItemForQueries { Strings = new[] { "Alpha", "Bravo", "Charlie" } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.Strings.QxAny(e => e == "Temp" || e == "Bravo"))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.Strings, refetched.Strings);
        }

        [Test]
        public void QueryIntArrayUsingQxAny_MatchingFirstElementInArray_ItemIsReturned()
        {
            var item = new QxItemForQueries { Integers = new[] { 1, 2, 3 } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.Integers.QxAny(e => e == 1))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.Integers, refetched.Integers);
        }

        [Test]
        public void QueryIntArrayUsingQxAny_SecondCriteriaMatchesMiddleElementInArray_ItemIsReturned()
        {
            var item = new QxItemForQueries { Integers = new[] { 1, 2, 3 } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.Integers.QxAny(e => e == -1 || e == 2))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.Integers, refetched.Integers);
        }

        [Test]
        public void QueryStringListUsingQxAny_MatchingFirstElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries { StringList = new List<string> { "Alpha", "Bravo", "Charlie" } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.StringList.QxAny(e => e == "Alpha"))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.StringList, refetched.StringList);
        }

        [Test]
        public void QueryStringListUsingQxAny_SecondCriteriaMatchesMiddleElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries { StringList = new List<string> { "Alpha", "Bravo", "Charlie" } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.StringList.QxAny(e => e == "Temp" || e == "Bravo"))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.StringList, refetched.StringList);
        }

        [Test]
        public void QueryDecimalListUsingQxAny_MatchingFirstElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries { DecimalList = new List<decimal> { -1.12M, 0.13M, 3.14M } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.DecimalList.QxAny(e => e == 0.13M))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.DecimalList, refetched.DecimalList);
        }

        [Test]
        public void QueryDecimalListUsingQxAny_SecondCriteriaMatchesMiddleElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries { DecimalList = new List<decimal> { -1.12M, 0.13M, 3.14M } };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.DecimalList.QxAny(e => e == -100 || e == 0.13M))).SingleOrDefault();
            }

            CollectionAssert.AreEqual(item.DecimalList, refetched.DecimalList);
        }

        [Test]
        public void QueryChildItemListUsingQxAny_MatchingPropOfFirstElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries
            {
                ChildItemList = new List<ChildItem> 
                {
                    new ChildItem { Int = -42 }, new ChildItem { Int = 42 }, new ChildItem { Int = 99 }
                }
            };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.ChildItemList.QxAny(e => e.Int == -42))).SingleOrDefault();
            }

            CustomAssert.AreEqual(item.ChildItemList, refetched.ChildItemList, (x, y) => x.Int.Equals(y.Int));
        }

        [Test]
        public void QueryChildItemListUsingQxAny_SecondCriteriaMatchesMiddleElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries
            {
                ChildItemList = new List<ChildItem> 
                {
                    new ChildItem { Int = -42 }, new ChildItem { Int = 42 }, new ChildItem { Int = 99 }
                }
            };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.ChildItemList.QxAny(e => e.Int == 0 || e.Int == 42))).SingleOrDefault();
            }

            CustomAssert.AreEqual(item.ChildItemList, refetched.ChildItemList, (x, y) => x.Int.Equals(y.Int));
        }

        [Test]
        public void QueryListOnChildItemListUsingQxAny_MatchingPropOfFirstElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries
            {
                ChildItemList = new List<ChildItem> {
                    new ChildItem { IntegerList = new List<int>{-1, 1, 2}},
                }
            };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.ChildItemList.QxAny(e => e.IntegerList.QxAny(e2 => e2 == -1)))).SingleOrDefault();
            }

            CustomAssert.AreEqual(item.ChildItemList, refetched.ChildItemList, (x, y) => x.Int.Equals(y.Int));
        }

        [Test]
        public void QueryListOnChildItemListUsingQxAny_SecondCriteriaMatchesMiddleElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries
            {
                ChildItemList = new List<ChildItem> {
                    new ChildItem { IntegerList = new List<int>{-1, 1, 2}},
                }
            };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.ChildItemList.QxAny(e => e.IntegerList.QxAny(e2 => e2 == -100 || e2 == 1)))).SingleOrDefault();
            }

            CustomAssert.AreEqual(item.ChildItemList, refetched.ChildItemList, (x, y) => x.Int.Equals(y.Int));
        }

        [Test]
        public void QueryGrandChildtemListUsingQxAny_MatchingPropOfFirstElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries
            {
                ChildItemList = new List<ChildItem> 
                {
                    new ChildItem 
                    { 
                        GrandChildItemList = new List<GrandChildItem>
                        {
                            new GrandChildItem {Int = -1}, new GrandChildItem {Int = 1}, new GrandChildItem {Int = 2},
                        }
                    },
                }
            };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i =>
                        i.ChildItemList.QxAny(e => 
                            e.GrandChildItemList.QxAny(e2 => e2.Int == -1)))).SingleOrDefault();
            }

            CustomAssert.AreEqual(
                item.ChildItemList[0].GrandChildItemList, 
                refetched.ChildItemList[0].GrandChildItemList,
                (x, y) => x.Int.Equals(y.Int));
        }

        [Test]
        public void QueryGrandChildtemListUsingQxAny_SecondCriteriaMatchesMiddleElementInList_ItemIsReturned()
        {
            var item = new QxItemForQueries
            {
                ChildItemList = new List<ChildItem> 
                {
                    new ChildItem 
                    { 
                        GrandChildItemList = new List<GrandChildItem>
                        {
                            new GrandChildItem {Int = -1}, new GrandChildItem {Int = 1}, new GrandChildItem {Int = 2},
                        }
                    },
                }
            };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => 
                        i.ChildItemList.QxAny(e => 
                            e.GrandChildItemList.QxAny(e2 => e2.Int == -100 || e2.Int == 1)))).SingleOrDefault();
            }

            CustomAssert.AreEqual(
                item.ChildItemList[0].GrandChildItemList,
                refetched.ChildItemList[0].GrandChildItemList,
                (x, y) => x.Int.Equals(y.Int));
        }

        private class QxItemForQueries
        {
            public Guid SisoId { get; set; }

            public string[] Strings { get; set; }

            public int[] Integers { get; set; }

            public IList<string> StringList { get; set; }

            public IList<decimal> DecimalList { get; set; }

            public IList<ChildItem> ChildItemList { get; set; }
        }

        private class ChildItem
        {
            public int Int { get; set; }

            public IList<int> IntegerList { get; set; }

            public IList<GrandChildItem> GrandChildItemList { get; set; }
        }

        private class GrandChildItem
        {
            public int Int { get; set; }
        }
    }
}