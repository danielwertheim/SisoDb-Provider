using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork.Queries
{
	class GetByIds
	{
		[Subject(typeof(ISession), "Get by Ids")]
		public class when_ids_matches_two_of_four_items_that_is_are_uncommitted_mode : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginSession())
				{
					session.InsertMany(_structures);

					_fetchedStructures = session.GetByIds<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
				}
			};

			It should_fetch_2_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_fetch_the_two_middle_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<QueryGuidItem> _fetchedStructures;
		}

		[Subject(typeof(ISession), "Get by Ids as Json")]
		public class when_ids_matches_two_of_four_json_items_that_are_in_uncommitted_mode : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginSession())
				{
					session.InsertMany(_structures);

					_fetchedStructures = session.GetByIdsAsJson<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
				}
			};

			It should_fetch_2_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_fetch_the_two_middle_structures = () =>
			{
				_fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
				_fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<string> _fetchedStructures;
		}
	}
}