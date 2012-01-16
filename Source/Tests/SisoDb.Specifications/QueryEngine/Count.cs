using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
	class Count
    {
        [Subject(typeof(IReadSession), "Count")]
        public class when_counting_all_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var rs = TestContext.Database.BeginReadSession())
                    _itemsCount = rs.Query<QueryGuidItem>().Count();
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(IReadSession), "Count")]
        public class when_counting_using_query_and_no_items_exists : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var rs = TestContext.Database.BeginReadSession())
					_itemsCount = rs.Query<QueryGuidItem>().Count(x => x.StringValue == "Goofy");
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }

        [Subject(typeof(IReadSession), "Count")]
        public class when_two_items_exists_in_committed_unit_of_work : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginWriteSession())
                {
                    session.InsertMany(new[]
                    {
                        new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                        new QueryGuidItem{SortOrder = 2, StringValue = "B"}
                    });
                }
            };

            Because of = () =>
            {
                using (var session =TestContext.Database.BeginReadSession())
                    _itemsCount = session.Query<QueryGuidItem>().Count();
            };

            It should_be_2 = () => _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(IReadSession), "Count")]
        public class when_query_matches_two_of_three_existing_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginWriteSession())
                {
                    session.InsertMany(new[]
                    {
                        new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                        new QueryGuidItem{SortOrder = 2, StringValue = "B"},
                        new QueryGuidItem{SortOrder = 3, StringValue = "C"}
                    });
                }
            };

            Because of = () =>
            {
                using (var session =TestContext.Database.BeginReadSession())
                    _itemsCount = session.Query<QueryGuidItem>().Count(x => x.SortOrder > 1);
            };

            It should_be_2 = () => _itemsCount.ShouldEqual(2);

            private static int _itemsCount;
        }

        [Subject(typeof(IReadSession), "Count")]
        public class when_query_matches_none_of_three_existing_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginWriteSession())
                {
                    session.InsertMany(new[]
                    {
                        new QueryGuidItem{SortOrder = 1, StringValue = "A"},
                        new QueryGuidItem{SortOrder = 2, StringValue = "B"},
                        new QueryGuidItem{SortOrder = 3, StringValue = "C"}
                    });
                }
            };

            Because of = () =>
            {
                using (var session =TestContext.Database.BeginReadSession())
                    _itemsCount = session.Query<QueryGuidItem>().Count(x => x.SortOrder < 1);
            };

            It should_be_0 = () => _itemsCount.ShouldEqual(0);

            private static int _itemsCount;
        }
    }
}