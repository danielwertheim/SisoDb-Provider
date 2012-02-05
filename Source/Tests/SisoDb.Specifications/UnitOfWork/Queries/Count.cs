using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork.Queries
{
	class Count
	{
		[Subject(typeof(ISession), "Count")]
		public class when_counting_using_expression_matching_two_of_four_items_that_are_in_uncommitted_mode : SpecificationBase
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

			It should_result_in_count_of_2 = () => _count.ShouldEqual(2);

			private static int _count;
		}
	}
}