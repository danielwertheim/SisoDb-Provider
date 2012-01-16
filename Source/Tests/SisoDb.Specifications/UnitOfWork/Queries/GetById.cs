using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork.Queries
{
	class GetById
	{
		[Subject(typeof(IWriteSession), "Get by Id (guid)")]
		public class when_set_with_guid_contains_four_items_that_are_in_uncommitted_mode : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.InsertMany(_structures);

					_fetchedStructure = session.GetById<QueryGuidItem>(_structures[1].StructureId);
				}
			};

			It should_fetch_the_structure =
				() => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

			private static IList<QueryGuidItem> _structures;
			private static QueryGuidItem _fetchedStructure;
		}
	}
}