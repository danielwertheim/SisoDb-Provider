using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
    namespace GetByIds
    {
        [Subject(typeof(IUnitOfWork), "Get by Ids")]
        public class when_id_set_matches_two_of_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => 
                _fetchedStructures = TestContext.Database.ReadOnce().GetByIds<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
            
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

        [Subject(typeof(IUnitOfWork), "Get by Ids")]
        public class when_id_set_matches_two_of_four_items_and_has_one_non_matching_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
                _nonMatchingId = Guid.Parse("DA2809E1-17A2-4D6C-8546-E2A86D29CF2B");
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.ReadOnce().GetByIds<QueryGuidItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Get by Ids as Json")]
        public class when_id_set_matches_two_of_four_json_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.ReadOnce().GetByIdsAsJson<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();

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

        [Subject(typeof(IUnitOfWork), "Get by Ids as Json")]
        public class when_id_set_matches_two_of_four_json_items_and_has_one_non_matching_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
                _nonMatchingId = Guid.Parse("81EC4983-F58B-4459-84F8-0D000F06F43D");
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.ReadOnce().GetByIdsAsJson<QueryGuidItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }
    }
}