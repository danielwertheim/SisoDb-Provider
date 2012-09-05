using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.SetFunctions
{
    namespace QxAny
    {
        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_strings_and_criteria_matches_first_element_in_array_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{Strings = new[]{"Alpha", "Bravo"}},
                    new QueryItemForQxAnyQueries{Strings = new[]{"Bravo", "Alpha"}},
                    new QueryItemForQxAnyQueries{Strings = new[]{"Charlie", "Delta"}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.Strings.QxAny(e => e == "Alpha")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_strings_and_second_criteria_matches_middle_element_in_array_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{Strings = new[]{"Alpha", "Bravo", "Charlie"}},
                    new QueryItemForQxAnyQueries{Strings = new[]{"Alpha", "Bravo", "Charlie"}},
                    new QueryItemForQxAnyQueries{Strings = new[]{"Charlie", "Delta", "Echo"}},
                    new QueryItemForQxAnyQueries{Strings = new[]{"Charlie", "Delta", "Echo"}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.Strings.QxAny(e => e == "Foo" || e == "Bravo")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_ints_and_criteria_matches_first_element_in_array_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{Integers = new[]{1, 2}},
                    new QueryItemForQxAnyQueries{Integers = new[]{2, 1}},
                    new QueryItemForQxAnyQueries{Integers = new[]{3, 4}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.Integers.QxAny(e => e == 1)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_ints_and_criteria_matches_middle_element_in_array_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{Integers = new[]{1, 2, 3}},
                    new QueryItemForQxAnyQueries{Integers = new[]{1, 2, 3}},
                    new QueryItemForQxAnyQueries{Integers = new[]{4, 5, 6}},
                    new QueryItemForQxAnyQueries{Integers = new[]{4, 5, 6}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.Integers.QxAny(e => e == 0 || e == 2)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_strings_and_criteria_matches_first_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{StringList = new[]{"Alpha", "Bravo"}},
                    new QueryItemForQxAnyQueries{StringList = new[]{"Bravo", "Alpha"}},
                    new QueryItemForQxAnyQueries{StringList = new[]{"Charlie", "Delta"}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.StringList.QxAny(e => e == "Alpha")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_strings_and_second_criteria_matches_middle_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{StringList = new[]{"Alpha", "Bravo", "Charlie"}},
                    new QueryItemForQxAnyQueries{StringList = new[]{"Alpha", "Bravo", "Charlie"}},
                    new QueryItemForQxAnyQueries{StringList = new[]{"Charlie", "Delta", "Echo"}},
                    new QueryItemForQxAnyQueries{StringList = new[]{"Charlie", "Delta", "Echo"}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.StringList.QxAny(e => e == "Foo" || e == "Bravo")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_decimals_and_criteria_matches_first_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{DecimalList = new[]{1.11m, 2.12m}},
                    new QueryItemForQxAnyQueries{DecimalList = new[]{2.12m, 1.11m}},
                    new QueryItemForQxAnyQueries{DecimalList = new[]{3.13m, 4.14m}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.DecimalList.QxAny(e => e == 1.11m)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_decimals_and_second_criteria_matches_middle_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries{DecimalList = new[]{1.11m, 2.12m, 3.13m}},
                    new QueryItemForQxAnyQueries{DecimalList = new[]{1.11m, 2.12m, 3.13m}},
                    new QueryItemForQxAnyQueries{DecimalList = new[]{4.14m, 5.15m, 6.16m}},
                    new QueryItemForQxAnyQueries{DecimalList = new[]{4.14m, 5.15m, 6.16m}},
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.DecimalList.QxAny(e => e == 0m || e == 2.12m)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_child_list_and_criteria_matches_first_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { Int = -10 }, new ChildItem { Int = 5 }, new ChildItem { Int = 10 }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { Int = -10 }, new ChildItem { Int = 5 }, new ChildItem { Int = 10 }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { Int = 100 }, new ChildItem { Int = 500 }, new ChildItem { Int = 1000 }
                        }
                    }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
               .Query<QueryItemForQxAnyQueries>().Where(i => i.ChildItemList.QxAny(e => e.Int == -10)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_child_list_and_second_criteria_matches_middle_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { Int = -10 }, new ChildItem { Int = 5 }, new ChildItem { Int = 10 }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { Int = -10 }, new ChildItem { Int = 5 }, new ChildItem { Int = 10 }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { Int = 100 }, new ChildItem { Int = 500 }, new ChildItem { Int = 1000 }
                        }
                    }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.ChildItemList.QxAny(e => e.Int == 0 || e.Int == 5)).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_list_on_child_list_and_criteria_matches_first_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { IntegerList = new[]{100, 500, 1000} }, new ChildItem { IntegerList = new[]{100, 500, 1000} }, new ChildItem { IntegerList = new[]{100, 500, 1000} }
                        }
                    }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.ChildItemList.QxAny(e => e.IntegerList.QxAny(il => il == -10))).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_list_on_child_list_and_second_criteria_matches_middle_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }, new ChildItem { IntegerList = new[]{-10, 5, 10} }
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { IntegerList = new[]{100, 500, 1000} }, new ChildItem { IntegerList = new[]{100, 500, 1000} }, new ChildItem { IntegerList = new[]{100, 500, 1000} }
                        }
                    }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.ChildItemList.QxAny(e => e.IntegerList.QxAny(il => il == 0 || il == 5))).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_grandchild_list_on_child_list_and_criteria_matches_first_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { GrandChildItemList = new []{new GrandChildItem{Int = -10}, new GrandChildItem{Int = 5}, new GrandChildItem{Int = 10}}}
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { GrandChildItemList = new []{new GrandChildItem{Int = -10}, new GrandChildItem{Int = 5}, new GrandChildItem{Int = 10}}}
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { GrandChildItemList = new []{new GrandChildItem{Int = 100}, new GrandChildItem{Int = 500}, new GrandChildItem{Int = 1000}}}
                        }
                    }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.ChildItemList.QxAny(e => e.GrandChildItemList.QxAny(gcl => gcl.Int == -10))).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_querying_grandchild_list_on_child_list_and_second_criteria_matches_middle_element_in_list_for_two_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { GrandChildItemList = new []{new GrandChildItem{Int = -10}, new GrandChildItem{Int = 5}, new GrandChildItem{Int = 10}}}
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { GrandChildItemList = new []{new GrandChildItem{Int = -10}, new GrandChildItem{Int = 5}, new GrandChildItem{Int = 10}}}
                        }
                    },
                    new QueryItemForQxAnyQueries
                    {
                        ChildItemList = new List<ChildItem> 
                        {
                            new ChildItem { GrandChildItemList = new []{new GrandChildItem{Int = 100}, new GrandChildItem{Int = 500}, new GrandChildItem{Int = 1000}}}
                        }
                    }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(i => i.ChildItemList.QxAny(e => e.GrandChildItemList.QxAny(gcl => gcl.Int == 0 || gcl.Int == 5))).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_qxany_combined_with_qxin_on_simplevaluetype_targetting_two_of_three_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{IntList = new List<int>{1,2,3}},
                    new QueryItemForQxAnyQueries{IntList = new List<int>{4,5,6}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(x => x.IntList.QxAny(y => y.QxIn(1, 6))).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_last_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_qxany_without_expression_and_two_empty_and_two_non_empty_int_collections_exist : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{IntList = new List<int>{1,2,3}},
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{IntList = new List<int>{4,5,6}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(x => x.IntList.QxAny()).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_structures_with_non_empty_collections = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_qxany_without_expression_and_two_empty_and_two_non_empty_nullableint_collections_exist : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{NullableIntsList = new List<int?>{1,2,3}},
                    new QueryItemForQxAnyQueries{NullableIntsList = new List<int?>{null,null,null}},
                    new QueryItemForQxAnyQueries{NullableIntsList = new List<int?>{4,5,6}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(x => x.NullableIntsList.QxAny()).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_structures_with_non_empty_collections = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_qxany_without_expression_and_two_empty_and_two_non_empty_string_collections_exist : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{StringList = new List<string>{"A", "B", "C"}},
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{StringList = new List<string>{"D", "E", "F"}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(x => x.StringList.QxAny()).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_structures_with_non_empty_collections = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }

        [Subject(typeof(ISession), "QxAny")]
        public class when_negated_qxany_without_expression_and_two_empty_and_two_non_empty_string_collections_exist : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{StringList = new List<string>{"A", "B", "C"}},
                    new QueryItemForQxAnyQueries(),
                    new QueryItemForQxAnyQueries{StringList = new List<string>{"D", "E", "F"}}
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryItemForQxAnyQueries>().Where(x => !x.StringList.QxAny()).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_structures_with_empty_collections = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryItemForQxAnyQueries> _structures;
            private static IList<QueryItemForQxAnyQueries> _fetchedStructures;
        }
    }
}