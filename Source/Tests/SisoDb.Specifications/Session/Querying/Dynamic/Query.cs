using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Dynamic;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.Dynamic
{
    class Query
    {
        [Subject(typeof(ISisoDynamicQueryable), "Query")]
        public class when_using_plain_bool_and_nullable_value_type_hasvalue_and_chained_wheres_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = null, BoolValue = true, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 1, BoolValue = false, StringValue = "One" },
					new QueryNullableItem { NullableInt = 42, BoolValue = true, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                using(var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Query(typeof(QueryNullableItem))
                        .Where("i => i.BoolValue && i.NullableInt.HasValue && i.NullableInt != null")
                        .Where("i => i.NullableInt == 42").ToList().Cast<QueryNullableItem>().ToList();
                }
            };

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_last_structure =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[2]);


            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoDynamicQueryable), "Query with Sort and Page")]
        public class when_query_skips_first_and_last_and_then_creates_pages_of_size_7_and_asks_for_the_last_page : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Query(typeof(QueryGuidItem))
                        .Where("i => i.SortOrder > 1 && i.SortOrder < 10")
                        .OrderBy("i => i.SortOrder")
                        .Page(1, 7).ToEnumerable().Cast<QueryGuidItem>().ToList(); 
                }
            };

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[8]);

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }
    }
}