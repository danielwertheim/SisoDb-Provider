using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork.Queries
{
	class QueryAll
	{
		[Subject(typeof(IUnitOfWork), "Get all")]
		public class when_set_with_guid_contains_four_items_that_are_in_uncommitted_mode : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
			};

			Because of = () =>
			{
				using (var uow = TestContext.Database.CreateUnitOfWork())
				{
					uow.InsertMany(_structures);

					_fetchedStructures = uow.Query<QueryGuidItem>().ToList();
				}
			};

			It should_fetch_all_4_structures =
				() => _fetchedStructures.Count.ShouldEqual(4);

			It should_fetch_the_two_middle_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
				_fetchedStructures[2].ShouldBeValueEqualTo(_structures[2]);
				_fetchedStructures[3].ShouldBeValueEqualTo(_structures[3]);
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<QueryGuidItem> _fetchedStructures;
		}

		[Subject(typeof(IUnitOfWork), "Get all as Json")]
		public class when_set_with_guid_contains_four_json_items_that_are_in_uncommitted_mode : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
			};

			Because of = () =>
			{
				using (var uow = TestContext.Database.CreateUnitOfWork())
				{
					uow.InsertMany(_structures);

					_fetchedStructures = uow.Query<QueryGuidItem>().ToListOfJson();
				}
			};

			It should_fetch_all_4_structures =
				() => _fetchedStructures.Count.ShouldEqual(4);

			It should_fetch_the_two_middle_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0].AsJson());
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[1].AsJson());
				_fetchedStructures[2].ShouldBeValueEqualTo(_structures[2].AsJson());
				_fetchedStructures[3].ShouldBeValueEqualTo(_structures[3].AsJson());
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<string> _fetchedStructures;
		}
	}
}