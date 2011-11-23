using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Querying;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine.StringFunctions
{
    namespace QxContains
    {
        [Subject(typeof(IQueryEngine), "QxContains")]
        public class when_two_items_has_string_that_does_not_match_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxContains("Foo")).ToList();

            It should_not_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "QxContains")]
        public class when_two_items_has_string_that_starts_with_queried_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxContains("Al")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "QxContains")]
        public class when_two_items_has_string_that_partially_matches_queried_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxContains("lp")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "QxContains")]
        public class when_two_items_has_string_that_ends_with_queried_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxContains("ha")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "QxContains")]
        public class when_two_items_has_string_that_completely_matches_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxContains("Alpha")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }
    }
}