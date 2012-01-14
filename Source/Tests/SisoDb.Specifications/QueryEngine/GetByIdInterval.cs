using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PineCone.Structures;
using SisoDb.Resources;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
	class GetByIdInterval
    {
        [Subject(typeof(IReadSession), "Get by Id interval")]
        public class when_getting_for_identities_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = 
                () => _result = TestContext.Database.ReadOnce().GetByIdInterval<QueryIdentityItem>(1, 3).ToList();

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static IList<QueryIdentityItem> _result;
        }

        [Subject(typeof(IReadSession), "Get by Id interval")]
        public class when_getting_for_big_identities_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of =
                () => _result = TestContext.Database.ReadOnce().GetByIdInterval<QueryBigIdentityItem>(1, 3).ToList();

            It should_return_empty_result = () => _result.Count.ShouldEqual(0);

            private static IList<QueryBigIdentityItem> _result;
        }

        [Subject(typeof(IReadSession), "Get by Id interval")]
        public class when_getting_for_guids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _idFrom = SequentialGuid.New();
                _idTo = Enumerable.Repeat(SequentialGuid.New(), 10).Last();
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    _result = TestContext.Database.ReadOnce().GetByIdInterval<QueryGuidItem>(_idFrom, _idTo).ToList();
                });
            };

            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
				CaughtException.ShouldBeOfType<SisoDbException>();

				var ex = (SisoDbException)CaughtException;
                ex.Message.ShouldContain(ExceptionMessages.ReadSession_GetByIdInterval_WrongIdType);
            };

            private static Guid _idFrom, _idTo;
            private static IList<QueryGuidItem> _result;
        }

        [Subject(typeof(IReadSession), "Get by Id interval")]
        public class when_getting_for_strings : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _idFrom = "A";
                _idTo = "P";
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    _result = TestContext.Database.ReadOnce().GetByIdInterval<QueryStringItem>(_idFrom, _idTo).ToList();
                });
            };

            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
				CaughtException.ShouldBeOfType<SisoDbException>();

				var ex = (SisoDbException)CaughtException;
                ex.Message.ShouldContain(ExceptionMessages.ReadSession_GetByIdInterval_WrongIdType);
            };

            private static string _idFrom, _idTo;
            private static IList<QueryStringItem> _result;
        }

        [Subject(typeof(IReadSession), "Get by Id interval")]
        public class when_getting_for_identities_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var uow = TestContext.Database.BeginWriteSession())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryIdentityItem{SortOrder = 1, StringValue = "A"},
                        new QueryIdentityItem{SortOrder = 2, StringValue = "B"},
                        new QueryIdentityItem{SortOrder = 3, StringValue = "C"},
                        new QueryIdentityItem{SortOrder = 4, StringValue = "D"},
                    });
                }
            };

            Because of = 
                () => _result = TestContext.Database.ReadOnce().GetByIdInterval<QueryIdentityItem>(2, 3).ToList();

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static IList<QueryIdentityItem> _result;
        }

        [Subject(typeof(IReadSession), "Get by Id interval")]
        public class when_getting_for_big_identities_and_range_matches_subset_of_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var uow = TestContext.Database.BeginWriteSession())
                {
                    uow.InsertMany(new[]
                    {
                        new QueryBigIdentityItem{SortOrder = 1, StringValue = "A"},
                        new QueryBigIdentityItem{SortOrder = 2, StringValue = "B"},
                        new QueryBigIdentityItem{SortOrder = 3, StringValue = "C"},
                        new QueryBigIdentityItem{SortOrder = 4, StringValue = "D"},
                    });
                }
            };

            Because of =
                () => _result = TestContext.Database.ReadOnce().GetByIdInterval<QueryBigIdentityItem>(2, 3).ToList();

            It should_have_subset_count_of_2 = () => _result.Count.ShouldEqual(2);

            It should_have_subset_containing_the_two_middle_items = () =>
            {
                _result.First().SortOrder.ShouldEqual(2);
                _result.Last().SortOrder.ShouldEqual(3);
            };

            private static IList<QueryBigIdentityItem> _result;
        }
    }
}