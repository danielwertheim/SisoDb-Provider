using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PineCone.Structures;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    namespace GetByIdInterval
    {
        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_identities_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<MyIdentityClass>(1, 3).ToList();
            };

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static IList<MyIdentityClass> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_big_identities_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<MyBigIdentityClass>(1, 3).ToList();
            };

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static IList<MyBigIdentityClass> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_guids_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _idFrom = SequentialGuid.NewSqlCompatibleGuid();
                _idTo = Enumerable.Repeat(SequentialGuid.NewSqlCompatibleGuid(), 10).Last();
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<MyGuidClass>(_idFrom, _idTo).ToList();
            };

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static Guid _idFrom, _idTo;
            private static IList<MyGuidClass> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_identities_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new MyIdentityClass{SortOrder = 1, Value = "A"},
                        new MyIdentityClass{SortOrder = 2, Value = "B"},
                        new MyIdentityClass{SortOrder = 3, Value = "C"},
                        new MyIdentityClass{SortOrder = 4, Value = "D"},
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<MyIdentityClass>(2, 3).ToList();
            };

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static IList<MyIdentityClass> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_big_identities_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[]
                    {
                        new MyBigIdentityClass{SortOrder = 1, Value = "A"},
                        new MyBigIdentityClass{SortOrder = 2, Value = "B"},
                        new MyBigIdentityClass{SortOrder = 3, Value = "C"},
                        new MyBigIdentityClass{SortOrder = 4, Value = "D"},
                    });

                    uow.Commit();
                }
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<MyBigIdentityClass>(2, 3).ToList();
            };

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static IList<MyBigIdentityClass> _result;
        }

        [Subject(typeof(Sql2008QueryEngine), "Get by Id interval")]
        public class when_getting_for_guids_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

                var items = new[]
                    {
                        new MyGuidClass{SortOrder = 1, Value = "A"},
                        new MyGuidClass{SortOrder = 2, Value = "B"},
                        new MyGuidClass{SortOrder = 3, Value = "C"},
                        new MyGuidClass{SortOrder = 4, Value = "D"},
                    };

                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }

                _idFrom = items[1].StructureId;
                _idTo = items[2].StructureId;
            };

            Because of = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                    _result = qe.GetByIdInterval<MyGuidClass>(_idFrom, _idTo).ToList();
            };

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static Guid _idFrom, _idTo;
            private static IList<MyGuidClass> _result;
        }

        public class MyGuidClass
        {
            public Guid StructureId { get; set; }

            public int SortOrder { get; set; }

            public string Value { get; set; }
        }

        public class MyIdentityClass
        {
            public int StructureId { get; set; }

            public int SortOrder { get; set; }

            public string Value { get; set; }
        }

        public class MyBigIdentityClass
        {
            public long StructureId { get; set; }

            public int SortOrder { get; set; }

            public string Value { get; set; }
        }
    }
}