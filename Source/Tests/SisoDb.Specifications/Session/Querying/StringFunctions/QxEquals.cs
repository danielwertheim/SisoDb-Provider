using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.StringFunctions
{
	class QxEquals
    {
        [Subject(typeof(ISession), "QxEquals")]
        public class when_two_items_has_string_that_does_not_match_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
					.Query<QueryGuidItem>().Where(i => i.StringValue.QxEquals("Foo")).ToList();

            It should_not_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxEquals")]
        public class when_two_items_has_string_that_partially_matches_queried_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.StringValue.QxEquals("lp")).ToList();

            It should_not_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        //[Subject(typeof(ISession), "QxLike")]
        //public class when_two_items_has_string_that_ends_with_queried_argument : SpecificationBase
        //{
        //    Establish context = () =>
        //    {
        //        TestContext = TestContextFactory.Create();
        //        _structures = QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
        //        {
        //            item.SortOrder = i + 1;
        //            item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
        //        });
        //        TestContext.Database.UseOnceTo().InsertMany(_structures);
        //    };

        //    Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
        //            .Query<QueryGuidItem>().Where(i => i.StringValue.QxLike("%ha")).ToList();

        //    It should_have_fetched_two_structures =
        //        () => _fetchedStructures.Count.ShouldEqual(2);

        //    It should_have_fetched_the_two_first_structures = () =>
        //    {
        //        _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
        //        _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
        //    };

        //    private static IList<QueryGuidItem> _structures;
        //    private static IList<QueryGuidItem> _fetchedStructures;
        //}

        [Subject(typeof(ISession), "QxEquals")]
        public class when_two_items_has_string_that_completely_matches_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                });
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
					.Query<QueryGuidItem>().Where(i => i.StringValue.QxEquals("Alpha")).ToList();

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