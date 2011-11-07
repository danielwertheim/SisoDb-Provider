using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
    namespace Query
    {
        [Subject(typeof(IQueryEngine), "Query")]
        public class when_expression_does_not_match_any_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Query<QueryGuidItem>(q => q.Where(i => i.SortOrder < 0)).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json")]
        public class when_expression_does_not_match_any_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .QueryAsJson<QueryGuidItem>(q => q.Where(i => i.SortOrder < 0)).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X")]
        public class when_expression_does_not_match_any_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .QueryAs<QueryGuidItem, QueryItemInfo>(q => q.Where(i => i.SortOrder < 0)).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Query<QueryGuidItem>(q => q.Where(i => i.SortOrder >= 2 && i.SortOrder <= 3)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .QueryAsJson<QueryGuidItem>(q => q.Where(i => i.SortOrder >= 2 && i.SortOrder <= 3)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .QueryAs<QueryGuidItem, QueryItemInfo>(q => q.Where(i => i.SortOrder >= 2 && i.SortOrder <= 3)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query")]
        public class when_expression_contains_item_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Query<QueryGuidItem>(q => q.Where(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json")]
        public class when_expression_contains_item_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .QueryAsJson<QueryGuidItem>(q => q.Where(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X")]
        public class when_expression_contains_item_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .QueryAs<QueryGuidItem, QueryItemInfo>(q => q.Where(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query")]
        public class when_expression_contains_local_constants_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
            {
                const int @from = 2;
                const int @to = 3;
                _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>(q => q.Where(i => i.SortOrder >= @from && i.SortOrder <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json")]
        public class when_expression_contains_local_constants_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
            {
                const int @from = 2;
                const int @to = 3;
                _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryGuidItem>(q => q.Where(i => i.SortOrder >= @from && i.SortOrder <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X")]
        public class when_expression_contains_local_constants_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
            {
                const int @from = 2;
                const int @to = 3;
                _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryGuidItem, QueryItemInfo>(q => q.Where(i => i.SortOrder >= @from && i.SortOrder <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query")]
        public class when_expression_contains_variables_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
            {
                var @from = 2;
                var @to = 3;
                _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>(q => q.Where(i => i.SortOrder >= @from && i.SortOrder <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json")]
        public class when_expression_contains_variables_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
            {
                var @from = 2;
                var @to = 3;
                _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryGuidItem>(q => q.Where(i => i.SortOrder >= @from && i.SortOrder <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X")]
        public class when_expression_contains_variables_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
            {
                var @from = 2;
                var @to = 3;
                _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryGuidItem, QueryItemInfo>(q => q.Where(i => i.SortOrder >= @from && i.SortOrder <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query (nested items)")]
        public class when_expression_contains_variables_matching_two_middle_nested_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryNestedGuidItem.CreateFourNestedItems());
            };

            Because of = () =>
            {
                var @from = 12;
                var @to = 13;
                _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryNestedGuidItem>(q => q.Where(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNestedGuidItem> _structures;
            private static IList<QueryNestedGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json (nested items)")]
        public class when_expression_contains_variables_matching_two_middle_nested_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryNestedGuidItem.CreateFourNestedItems());
            };

            Because of = () =>
            {
                var @from = 12;
                var @to = 13;
                _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryNestedGuidItem>(q => q.Where(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryNestedGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X (nested items)")]
        public class when_expression_contains_variables_matching_two_middle_nested_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryNestedGuidItem.CreateFourNestedItems());
            };

            Because of = () =>
            {
                var @from = 12;
                var @to = 13;
                _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryNestedGuidItem, QueryNestedItemInfo>(q => q.Where(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to)).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryNestedGuidItem> _structures;
            private static IList<QueryNestedItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query with Take")]
        public class when_query_matches_the_three_last_structures_of_four_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>(q => 
                q.Where(i => i.SortOrder >= 2 && i.SortOrder <= 4).Take(2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json with Take")]
        public class when_query_matches_the_three_last_json_structures_of_four_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryGuidItem>(q => 
                q.Where(i => i.SortOrder >= 2 && i.SortOrder <= 4).Take(2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X with Take")]
        public class when_query_matches_the_three_last_structures_of_four_and_take_is_two_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryGuidItem, QueryItemInfo>(
                q => q.Where(i => i.SortOrder >= 2 && i.SortOrder <= 4).Take(2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query with Take and Sort")]
        public class when_set_is_unsorted_and_query_matches_three_of_four_structures_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>(q =>
                q.Where(i => i.SortOrder == 2 || (i.SortOrder == 1 && i.StringValue == "B")).Take(2).SortBy(i => i.StringValue)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_inserted_structures_but_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[0]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json with Take and Sort")]
        public class when_set_is_unsorted_and_query_matches_three_of_four_json_structures_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryGuidItem>(q =>
                q.Where(i => i.SortOrder == 2 || (i.SortOrder == 1 && i.StringValue == "B")).Take(2).SortBy(i => i.StringValue)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_inserted_structures_but_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[0].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X with Take and Sort")]
        public class when_set_is_unsorted_and_query_matches_three_of_four_structures_and_take_is_two_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryGuidItem, QueryItemInfo>(q =>
                q.Where(i => i.SortOrder == 2 || (i.SortOrder == 1 && i.StringValue == "B")).Take(2).SortBy(i => i.StringValue, i => i.SortOrder)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_inserted_structures_but_in_reverse = () =>
            {
                _fetchedStructures[0].Matches(_structures[2]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[1]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query with Take, Sort and Page")]
        public class when_query_skips_first_and_last_and_then_takes_6_structures_and_creates_pages_of_2_and_asks_for_page_2 : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateTenItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>(q => q
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .SortBy(i => i.SortOrder)
                .Take(6)
                .Page(1, 2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_fourth_and_fifth_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[3]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[4]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json with Take, Sort and Page")]
        public class when_query_skips_first_and_last_and_then_takes_6_structures_and_creates_pages_of_2_and_asks_for_page_2_as_json : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateTenItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryGuidItem>(q => q
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .SortBy(i => i.SortOrder)
                .Take(6)
                .Page(1, 2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_fourth_and_fifth_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[3].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[4].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X with Take, Sort and Page")]
        public class when_query_skips_first_and_last_and_then_takes_6_structures_and_creates_pages_of_2_and_asks_for_page_2_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateTenItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryGuidItem, QueryItemInfo>(q => q
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .SortBy(i => i.SortOrder)
                .Take(6)
                .Page(1, 2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_fourth_and_fifth_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[3]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[4]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query with Take, Sort and Page")]
        public class when_query_skips_first_and_last_and_then_takes_5_structures_and_creates_pages_of_2_and_asks_for_last_page : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateTenItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>(q => q
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .SortBy(i => i.SortOrder)
                .Take(5)
                .Page(2, 2)).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure = 
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[5]);

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as Json with Take, Sort and Page")]
        public class when_query_skips_first_and_last_and_then_takes_5_structures_and_creates_pages_of_2_and_asks_for_last_page_as_json : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateTenItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAsJson<QueryGuidItem>(q => q
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .SortBy(i => i.SortOrder)
                .Take(5)
                .Page(2, 2)).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure = 
                () => _fetchedStructures[0].ShouldEqual(_structures[5].AsJson());

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Query as X with Take, Sort and Page")]
        public class when_query_skips_first_and_last_and_then_takes_5_structures_and_creates_pages_of_2_and_asks_for_last_page_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateTenItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce().QueryAs<QueryGuidItem, QueryItemInfo>(q => q
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .SortBy(i => i.SortOrder)
                .Take(5)
                .Page(2, 2)).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure =
                () => _fetchedStructures[0].Matches(_structures[5]).ShouldBeTrue();

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }
    }
}