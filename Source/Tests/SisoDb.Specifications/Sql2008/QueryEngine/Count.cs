using System;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    namespace Count
    {
        [Subject(typeof(Sql2008QueryEngine), "Count")]
        public class when_counting_all_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _itemsCount = qe.Count<QueryGuidItem>();
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(Sql2008QueryEngine), "Count")]
        public class when_counting_using_query_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _itemsCount = qe.Count<QueryGuidItem>(x => x.StringValue == "Goofy");
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(Sql2008QueryEngine), "Count")]
        public class when_two_items_exists_in_uncommitted_unit_of_work : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using(var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryGuidItem{IntValue = 1, StringValue = "A"},
                        new QueryGuidItem{IntValue = 2, StringValue = "B"}
                    });
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _itemsCount = qe.Count<QueryGuidItem>();
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(Sql2008QueryEngine), "Count")]
        public class when_two_items_exists_in_committed_unit_of_work : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryGuidItem{IntValue = 1, StringValue = "A"},
                        new QueryGuidItem{IntValue = 2, StringValue = "B"}
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _itemsCount = qe.Count<QueryGuidItem>();
            };

            It should_be_2 = () => _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(Sql2008QueryEngine), "Count")]
        public class when_query_matches_two_of_three_existing_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryGuidItem{IntValue = 1, StringValue = "A"},
                        new QueryGuidItem{IntValue = 2, StringValue = "B"},
                        new QueryGuidItem{IntValue = 3, StringValue = "C"}
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _itemsCount = qe.Count<QueryGuidItem>(x => x.IntValue > 1);
            };

            It should_be_2 = () => _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(Sql2008QueryEngine), "Count")]
        public class when_query_matches_none_of_three_existing_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryGuidItem{IntValue = 1, StringValue = "A"},
                        new QueryGuidItem{IntValue = 2, StringValue = "B"},
                        new QueryGuidItem{IntValue = 3, StringValue = "C"}
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _itemsCount = qe.Count<QueryGuidItem>(x => x.IntValue < 1);
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }
    }
}