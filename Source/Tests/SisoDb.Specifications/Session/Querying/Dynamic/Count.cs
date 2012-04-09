using Machine.Specifications;
using SisoDb.Dynamic;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.Dynamic
{
    class Count
    {
        [Subject(typeof(ISisoDynamicQueryable), "Count")]
        public class when_no_expression_is_specified_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _itemsCount = session.Query(typeof(QueryGuidItem)).Count();
                }
            };


            It should_be_0 = () =>
                _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(ISisoDynamicQueryable), "Count")]
        public class when_no_expression_is_specified_and_two_items_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(new[]
                {
                    new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                    new QueryGuidItem{SortOrder = 2, StringValue = "B"}
                });
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _itemsCount = session.Query(typeof(QueryGuidItem)).Count();
                }
            };

            It should_be_2 = () =>
                _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(ISisoDynamicQueryable), "Count")]
        public class when_expression_is_specified_and_it_matches_two_of_three_existing_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(new[]
                {
                    new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                    new QueryGuidItem{SortOrder = 2, StringValue = "B"},
                    new QueryGuidItem{SortOrder = 3, StringValue = "C"}
                });
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _itemsCount = session.Query(typeof(QueryGuidItem)).Count("x => x.SortOrder > 1");
                }
            };

            It should_be_2 = () =>
                _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }
    }
}