using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class Any
    {
        [Subject(typeof(ISession), "Any")]
        public class when_no_expression_is_specified_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create();

            Because of = () =>
                _result = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Any();


            It should_be_false= () =>
                _result.ShouldBeFalse();

            private static bool _result;
        }

        [Subject(typeof(ISession), "Any")]
        public class when_expression_is_specified_and_no_items_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create();

            Because of = () =>
                _result = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Any(x => x.StringValue == "Goofy");

            It should_be_false = () =>
                _result.ShouldBeFalse();

            private static bool _result;
        }

        [Subject(typeof(ISession), "Any")]
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
                _result = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Any();
            
            It should_be_true = () =>
                _result.ShouldBeTrue();

            private static bool _result;
        }

        [Subject(typeof(ISession), "Any")]
        public class when_expression_is_specified_and_it_matches_two_of_four_items_that_are_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());

                    _result = session.Query<QueryGuidItem>().Any(x => x.SortOrder >= 3);
                }
            };

            It should_be_true = () =>
                _result.ShouldBeTrue();

            private static bool _result;
        }

        [Subject(typeof(ISession), "Any")]
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
                _result = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Any(x => x.SortOrder > 1);

            It should_be_true = () =>
                _result.ShouldBeTrue();

            private static bool _result;
        }

        [Subject(typeof(ISession), "Any")]
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
                _result = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Any(x => x.SortOrder < 1);

            It should_be_false = () =>
                _result.ShouldBeFalse();

            private static bool _result;
        }
    }
}