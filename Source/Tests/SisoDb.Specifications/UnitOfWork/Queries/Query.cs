using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork.Queries
{
	class Query
    {
        [Subject(typeof(IWriteSession), "Query")]
        public class when_query_matches_two_of_four_items_that_are_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.BeginWriteSession())
                {
                    uow.InsertMany(_structures);

                    _fetchedStructures = uow.Query<QueryGuidItem>().Where(x => x.SortOrder >= _structures[1].SortOrder && x.SortOrder <= _structures[2].SortOrder).ToList();
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
    }
}