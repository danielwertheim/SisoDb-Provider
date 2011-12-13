using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.ReadSession
{
	class QueryAll
    {
        [Subject(typeof(IUnitOfWork), "Query all")]
        public class when_set_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();
            
            Because of = () =>
                _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().ToList();

            It should_fetch_0_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all as Json")]
        public class when_set_of_json_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
				_fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().ToListOfJson();

            It should_fetch_0_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all")]
        public class when_set_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().ToList();

            It should_fetch_all_4_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_fetch_all_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[2].ShouldBeValueEqualTo(_structures[2]);
                _fetchedStructures[3].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all as Json")]
        public class when_set_contains_four_json_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().ToListOfJson();

            It should_fetch_all_4_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_fetch_all_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0].AsJson());
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1].AsJson());
                _fetchedStructures[2].ShouldBeValueEqualTo(_structures[2].AsJson());
                _fetchedStructures[3].ShouldBeValueEqualTo(_structures[3].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all, with sorting")]
        public class when_set_contains_four_items_inserted_unordered : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().OrderBy(i => i.SortOrder).ToList();

            It should_fetch_all_4_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_fetch_all_structures_in_correct_order = () =>
            {
                var orderedStructures = _structures.OrderBy(s => s.SortOrder).ToArray();

                for (var c = 0; c < 4; c++)
                    _fetchedStructures[c].ShouldBeValueEqualTo(orderedStructures[c]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all as Json, with sorting")]
        public class when_set_contains_four_json_items_inserted_unordered : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().OrderBy(i => i.SortOrder).ToListOfJson();

            It should_fetch_all_4_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_fetch_all_structures_in_correct_order = () =>
            {
                var orderedStructures = _structures.OrderBy(s => s.SortOrder).Select(s => s.AsJson()).ToArray();

                for (var c = 0; c < 4; c++)
                    _fetchedStructures[c].ShouldEqual(orderedStructures[c]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all, with multiple sorting")]
        public class when_set_contains_four_items_inserted_unordered_and_sorted_by_two_criterias : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().OrderBy(i => i.SortOrder, i => i.StringValue).ToList();

            It should_fetch_all_4_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_fetch_all_structures_in_correct_order = () =>
            {
                var orderedStructures = _structures.OrderBy(s => s.SortOrder).OrderBy(s => s.StringValue).ToArray();

                for (var c = 0; c < 4; c++)
                    _fetchedStructures[c].ShouldBeValueEqualTo(orderedStructures[c]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IUnitOfWork), "Query all, with sorting")]
        public class when_set_contains_four_items_inserted_unordered_and_sorting_on_different_members_but_same_datatype : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce().Query<QueryGuidItem>().OrderBy(i => i.SortOrder, i => i.IntegerValue).ToList();

            It should_fetch_all_4_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_fetch_all_structures_in_correct_order = () =>
            {
                var orderedStructures = _structures.OrderBy(s => s.SortOrder).OrderBy(s => s.IntegerValue).ToArray();

                for (var c = 0; c < 4; c++)
                    _fetchedStructures[c].ShouldBeValueEqualTo(orderedStructures[c]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }
    }
}