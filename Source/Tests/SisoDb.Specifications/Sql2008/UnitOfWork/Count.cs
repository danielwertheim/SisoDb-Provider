using System;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.UnitOfWork
{
    namespace Count
    {
        [Subject(typeof(Sql2008UnitOfWork), "Count")]
        public class when_counting_on_item_that_does_not_exist : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                    _count = uow.Count<QueryGuidItem>();
            };

            It should_result_in_count_of_0 = () => _count.ShouldEqual(0);

            private static int _count;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Count")]
        public class when_counting_using_expression_on_item_that_does_not_exist : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                    _count = uow.Count<QueryGuidItem>(x => x.IntValue == 0);
            };

            It should_result_in_count_of_0 = () => _count.ShouldEqual(0);

            private static int _count;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Count")]
        public class when_counting_on_three_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.UoW().InsertMany(new[]
                {
                    new QueryGuidItem { IntValue = 1 },
                    new QueryGuidItem { IntValue = 2 },
                    new QueryGuidItem { IntValue = 3 }
                });
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                    _count = uow.Count<QueryGuidItem>();
            };

            It should_result_in_count_of_3 = () => _count.ShouldEqual(3);

            private static int _count;
        }
        
        [Subject(typeof(Sql2008UnitOfWork), "Count")]
        public class when_counting_using_expression_matching_all_three_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.UoW().InsertMany(new[]
                {
                    new QueryGuidItem { IntValue = 1 },
                    new QueryGuidItem { IntValue = 2 },
                    new QueryGuidItem { IntValue = 3 }
                });
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                    _count = uow.Count<QueryGuidItem>(x => x.IntValue > 0);
            };

            It should_result_in_count_of_3 = () => _count.ShouldEqual(3);

            private static int _count;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Count")]
        public class when_counting_using_expression_matching_two_of_three_items_that_is_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryGuidItem{IntValue = 1},
                        new QueryGuidItem{IntValue = 2},
                        new QueryGuidItem{IntValue = 3}
                    });

                    _count = uow.Count<QueryGuidItem>(x => x.IntValue >= 2);
                }
            };

            It should_result_in_count_of_2 = () => _count.ShouldEqual(2);

            private static int _count;
        }

        public class QueryGuidItem
        {
            public Guid StructureId { get; set; }

            public int IntValue { get; set; }

            public string StringValue { get; set; }
        }
    }
}