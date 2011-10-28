using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using NCore;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.UnitOfWork
{
    namespace Queries
    {
        [Subject(typeof(Sql2008UnitOfWork), "Count")]
        public class when_counting_using_expression_matching_two_of_four_items_that_is_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(QueryGuidItem.CreateItems());

                    _count = uow.Count<QueryGuidItem>(x => x.IntValue >= 3);
                }
            };

            It should_result_in_count_of_2 = () => _count.ShouldEqual(2);

            private static int _count;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Get by Id interval")]
        public class when_id_interval_matches_two_of_four_items_that_is_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = QueryGuidItem.CreateItems();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(_structures);

                    _fetchedStructures = uow.GetByIdInterval<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
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

        [Subject(typeof(Sql2008UnitOfWork), "Get by Ids")]
        public class when_id_set_matches_two_of_four_items_that_is_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = QueryGuidItem.CreateItems();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(_structures);

                    _fetchedStructures = uow.GetByIds<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
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

        [Subject(typeof(Sql2008UnitOfWork), "Get by Ids as Json")]
        public class when_id_set_matches_two_of_four_json_items_that_is_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = QueryGuidItem.CreateItems();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(_structures);

                    _fetchedStructures = uow.GetByIdsAsJson<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
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

        public class QueryGuidItem
        {
            public const string JsonFormat = "{{\"StructureId\":\"{0}\",\"IntValue\":{1},\"StringValue\":\"{2}\"}}";

            public static IList<QueryGuidItem> CreateItems()
            {
                return new[]
                {
                    new QueryGuidItem{IntValue = 1, StringValue = "A"},
                    new QueryGuidItem{IntValue = 2, StringValue = "B"},
                    new QueryGuidItem{IntValue = 3, StringValue = "C"},
                    new QueryGuidItem{IntValue = 4, StringValue = "D"},
                };
            }

            public Guid StructureId { get; set; }

            public int IntValue { get; set; }

            public string StringValue { get; set; }

            public string AsJson()
            {
                return JsonFormat.Inject(StructureId.ToString("N"), IntValue, StringValue);
            }
        }
    }
}