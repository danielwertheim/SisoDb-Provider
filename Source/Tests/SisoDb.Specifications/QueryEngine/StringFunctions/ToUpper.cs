using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using NCore.Collections;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine.StringFunctions
{
    namespace ToUpper
    {
        //COLLATION FUCKS IT UP
		//[Subject(typeof(IQueryEngine), "ToUpper")]
        //public class when_query_does_not_match_any_items : SpecificationBase
        //{
        //    Establish context = () =>
        //    {
        //        TestContext = TestContextFactory.Create();
        //        TestContext.Database.DbWriteOnce().InsertMany(StringFunctionsItem.CreateItems(5, "FOO").ToList());
        //    };

        //    Because of = 
        //        () => _fetchedStructures = TestContext.Database.DbReadOnce().Where<StringFunctionsItem>(i => i.String1.ToUpper() == "foo1").ToList();

        //    It should_not_have_fetched_any_structures =
        //        () => _fetchedStructures.Count.ShouldEqual(0);

        //    private static IList<StringFunctionsItem> _fetchedStructures;
        //}

        [Subject(typeof(IReadSession), "ToUpper")]
        public class when_query_matches_subset_of_2_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.WriteOnce().InsertMany(StringFunctionsItem.CreateItems(5, "ABC").MergeWith(StringFunctionsItem.CreateItems(5, "EFG")).ToList());
            };

            Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<StringFunctionsItem>().Where(i => i.String1.ToUpper() == "EFG2" || i.String1.ToUpper() == "EFG3").ToList();

            It should_have_fetched_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_2_structures_matching_the_query = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[7]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[8]);
            };

            private static IList<StringFunctionsItem> _structures;
            private static IList<StringFunctionsItem> _fetchedStructures;
        }
    }
}