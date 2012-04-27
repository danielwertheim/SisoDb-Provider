using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class Count
    {
        [Subject(typeof(ISisoQueryable<>), "Count")]
        public class when_no_expression_is_specified_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create();

            Because of = () =>
                _itemsCount = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Count();

            It should_be_0 = () =>
                _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(ISisoQueryable<>), "Count")]
        public class when_expression_is_specified_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create();

            Because of = () =>
                _itemsCount = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Count(x => x.StringValue == "Goofy");

            It should_be_0 = () =>
                _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(ISisoQueryable<>), "Count")]
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
                _itemsCount = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Count();
            
            It should_be_2 = () => 
                _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(ISisoQueryable<>), "Count")]
        public class when_expression_is_specified_and_it_matches_two_of_four_items_that_are_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());

                    _count = session.Query<QueryGuidItem>().Count(x => x.SortOrder >= 3);
                }
            };

            It should_be_2 = () => _count.ShouldEqual(2);

            private static int _count;
        }

        [Subject(typeof(ISisoQueryable<>), "Count")]
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
                _itemsCount = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Count(x => x.SortOrder > 1);

            It should_be_2 = () => 
                _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(ISisoQueryable<>), "Count")]
        public class when_expression_is_specified_and_it_matches_none_of_three_existing_items : SpecificationBase
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
                _itemsCount = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Count(x => x.SortOrder < 1);

            It should_be_0 = () => 
                _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }
    }
}