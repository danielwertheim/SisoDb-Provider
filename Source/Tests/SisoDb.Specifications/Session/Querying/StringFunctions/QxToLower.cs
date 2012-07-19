using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.NCore.Collections;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.StringFunctions
{
    class QxToLower
    {
        //COLLATION FUCKS IT UP
		//[Subject(typeof(IQueryEngine), "ToLower")]
        //public class when_query_does_not_match_any_items : SpecificationBase
        //{
        //    Establish context = () =>
        //    {
        //        TestContext = TestContextFactory.Create();
        //        TestContext.Database.DbWriteOnce().InsertMany(StringFunctionsItem.CreateItems(5, "FOO").ToList());
        //    };

        //    Because of = 
        //        () => _fetchedStructures = TestContext.Database.UseOnceTo().Where<StringFunctionsItem>(i => i.String1.QxToLower() == "FOO1").ToList();

        //    It should_not_have_fetched_any_structures =
        //        () => _fetchedStructures.Count.ShouldEqual(0);

        //    private static IList<StringFunctionsItem> _fetchedStructures;
        //}

        [Subject(typeof(ISession), "QxToLower")]
        public class when_query_matches_subset_of_2_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = StringFunctionsItem.CreateItems(5, "ABC").MergeWith(StringFunctionsItem.CreateItems(5, "EFG")).ToList();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = 
				() => _fetchedStructures = TestContext.Database.UseOnceTo()
					.Query<StringFunctionsItem>()
					.Where(i => i.String1.QxToLower() == "efg2" || i.String1.QxToLower() == "efg3").ToList();

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

        [Subject(typeof(ISession), "QxToLower")]
        public class when_applied_on_inline_value_and_matches_subset_of_2_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = StringFunctionsItem.CreateItems(5, "ABC").MergeWith(StringFunctionsItem.CreateItems(5, "EFG")).ToList();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<StringFunctionsItem>().Where(i => i.String1.QxToLower() == "EFG2".QxToLower() || i.String1.QxToLower() == "EFG3".QxToLower()).ToList();

            It should_have_fetched_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_2_structures_matching_the_query = () =>
            {
                _fetchedStructures[0].String1.ShouldEqual("EFG2");
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[7]);

                _fetchedStructures[1].String1.ShouldEqual("EFG3");
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[8]);
            };

            private static IList<StringFunctionsItem> _structures;
            private static IList<StringFunctionsItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxToLower")]
        public class when_applied_inline_on_variable_value_and_matches_subset_of_2_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = StringFunctionsItem.CreateItems(5, "ABC").MergeWith(StringFunctionsItem.CreateItems(5, "EFG")).ToList();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var criteria1 = "EFG2";
                var criteria2 = "EFG3";
                _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<StringFunctionsItem>()
                    .Where(i => i.String1.QxToLower() == criteria1.QxToLower() || i.String1.QxToLower() == criteria2.QxToLower()).ToList();
            };

            It should_have_fetched_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_2_structures_matching_the_query = () =>
            {
                _fetchedStructures[0].String1.ShouldEqual("EFG2");
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[7]);

                _fetchedStructures[1].String1.ShouldEqual("EFG3");
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[8]);
            };

            private static IList<StringFunctionsItem> _structures;
            private static IList<StringFunctionsItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxToLower")]
        public class when_applied_on_variable_value_and_matches_subset_of_2_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = StringFunctionsItem.CreateItems(5, "ABC").MergeWith(StringFunctionsItem.CreateItems(5, "EFG")).ToList();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var criteria1 = "EFG2".QxToLower();
                var criteria2 = "EFG3".QxToLower();
                _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<StringFunctionsItem>()
                    .Where(i => i.String1.QxToLower() == criteria1 || i.String1.QxToLower() == criteria2).ToList();
            };

            It should_have_fetched_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_2_structures_matching_the_query = () =>
            {
                _fetchedStructures[0].String1.ShouldEqual("EFG2");
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[7]);

                _fetchedStructures[1].String1.ShouldEqual("EFG3");
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[8]);
            };

            private static IList<StringFunctionsItem> _structures;
            private static IList<StringFunctionsItem> _fetchedStructures;
        }
    }
}