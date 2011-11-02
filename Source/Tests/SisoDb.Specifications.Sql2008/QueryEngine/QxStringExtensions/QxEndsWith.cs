using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Querying;
using SisoDb.Specifications.Sql2008.Model;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.QueryEngine.QxStringExtensions
{
    namespace QxEndsWith
    {
        [Subject(typeof(Sql2008QueryEngine), "QxEndsWith")]
        public class when_two_items_has_string_that_does_not_match_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxEndsWith("Foo")).ToList();

            It should_not_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(Sql2008QueryEngine), "QxEndsWith")]
        public class when_two_items_has_string_that_ends_with_queried_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxEndsWith("ha")).ToList();

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

        [Subject(typeof(Sql2008QueryEngine), "QxEndsWith")]
        public class when_two_items_has_string_that_completely_matches_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                    .Where<QueryGuidItem>(i => i.StringValue.QxEndsWith("Alpha")).ToList();

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