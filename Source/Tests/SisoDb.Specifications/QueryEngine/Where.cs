using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
    namespace Where
    {
        [Subject(typeof(IQueryEngine), "Where")]
        public class when_expression_does_not_match_any_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.SortOrder < 0).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Where as Json")]
        public class when_expression_does_not_match_any_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .WhereAsJson<QueryGuidItem>(i => i.SortOrder < 0).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Where as X")]
        public class when_expression_does_not_match_any_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .WhereAs<QueryGuidItem, QueryItemInfo>(i => i.SortOrder < 0).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Where")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.SortOrder >= 2 && i.SortOrder <= 3).ToList();

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

        [Subject(typeof(IQueryEngine), "Where as Json")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .WhereAsJson<QueryGuidItem>(i => i.SortOrder >= 2 && i.SortOrder <= 3).ToList();

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

        [Subject(typeof(IQueryEngine), "Where as X")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .WhereAs<QueryGuidItem, QueryItemInfo>(i => i.SortOrder >= 2 && i.SortOrder <= 3).ToList();

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

        [Subject(typeof(IQueryEngine), "Where")]
        public class when_expression_contains_item_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder).ToList();

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

        [Subject(typeof(IQueryEngine), "Where as Json")]
        public class when_expression_contains_item_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .WhereAsJson<QueryGuidItem>(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder).ToList();

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

        [Subject(typeof(IQueryEngine), "Where as X")]
        public class when_expression_contains_item_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .WhereAs<QueryGuidItem, QueryItemInfo>(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder).ToList();

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

        [Subject(typeof(IQueryEngine), "Where")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().Where<QueryGuidItem>(i => i.SortOrder >= @from && i.SortOrder <= to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where as Json")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().WhereAsJson<QueryGuidItem>(i => i.SortOrder >= @from && i.SortOrder <= to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where as X")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().WhereAs<QueryGuidItem, QueryItemInfo>(i => i.SortOrder >= @from && i.SortOrder <= to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().Where<QueryGuidItem>(i => i.SortOrder >= @from && i.SortOrder <= @to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where as Json")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().WhereAsJson<QueryGuidItem>(i => i.SortOrder >= @from && i.SortOrder <= @to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where as X")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().WhereAs<QueryGuidItem, QueryItemInfo>(i => i.SortOrder >= @from && i.SortOrder <= @to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where (nested items)")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().Where<QueryNestedGuidItem>(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where as Json (nested items)")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().WhereAsJson<QueryNestedGuidItem>(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to).ToList();
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

        [Subject(typeof(IQueryEngine), "Where as X (nested items)")]
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
                _fetchedStructures = TestContext.Database.ReadOnce().WhereAs<QueryNestedGuidItem, QueryNestedItemInfo>(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to).ToList();
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
    }
}