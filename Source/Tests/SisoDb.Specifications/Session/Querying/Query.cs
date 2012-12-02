using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Dynamic;
using SisoDb.NCore;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class Query
    {
        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_plain_bool_and_nullable_int_hasvalue_and_chained_wheres_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = null, BoolValue = true, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 1, BoolValue = false, StringValue = "One" },
                    new QueryNullableItem { NullableInt = 41, BoolValue = true, StringValue = "Fourthy one"},
					new QueryNullableItem { NullableInt = 42, BoolValue = true, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.BoolValue && i.NullableInt.HasValue).Where(i => i.NullableInt == 42).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_true_bool_and_non_null_int_where_int_is_42 =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[3]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_plain_bool_and_nullable_int_notnull_and_chained_wheres_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = null, BoolValue = true, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 1, BoolValue = false, StringValue = "One" },
                    new QueryNullableItem { NullableInt = 41, BoolValue = true, StringValue = "Fourthy one"},
					new QueryNullableItem { NullableInt = 42, BoolValue = true, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.BoolValue && i.NullableInt != null).Where(i => i.NullableInt == 42).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_true_bool_and_non_null_int_where_int_is_42 =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[3]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_int_hasvalue_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = 1, StringValue = "One" },
                    new QueryNullableItem { NullableInt = null, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 42, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableInt.HasValue).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_an_int = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_int_notnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = 1, StringValue = "One" },
                    new QueryNullableItem { NullableInt = null, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 42, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableInt != null).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_an_int = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_int_not_hasvalue_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = 1, StringValue = "One" },
                    new QueryNullableItem { NullableInt = null, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 42, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableInt.HasValue == false).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_a_nullableint_being_null = 
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_int_isnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = 1, StringValue = "One" },
                    new QueryNullableItem { NullableInt = null, StringValue = "Null" },
					new QueryNullableItem { NullableInt = 42, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableInt == null).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_a_nullableint_being_null = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_decimal_hasvalue_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDecimal = 1M, StringValue = "One" },
                    new QueryNullableItem { NullableInt = null, StringValue = "Null" },
					new QueryNullableItem { NullableDecimal = 42M, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDecimal.HasValue).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_an_int = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_decimal_notnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDecimal = 1M, StringValue = "One" },
                    new QueryNullableItem { NullableDecimal = null, StringValue = "Null" },
					new QueryNullableItem { NullableDecimal = 42M, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDecimal != null).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_an_int = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_decimal_not_hasvalue_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDecimal = 1M, StringValue = "One" },
                    new QueryNullableItem { NullableDecimal = null, StringValue = "Null" },
					new QueryNullableItem { NullableDecimal = 42M, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDecimal.HasValue == false).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_a_nullableint_being_null =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_decimal_isnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDecimal = 1M, StringValue = "One" },
                    new QueryNullableItem { NullableDecimal = null, StringValue = "Null" },
					new QueryNullableItem { NullableDecimal = 42M, StringValue = "Fourthy two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDecimal == null).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_a_nullableint_being_null = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_datetime_hasvalue_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "One" },
                    new QueryNullableItem { NullableDateTime = null, StringValue = "Null" },
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "Two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDateTime.HasValue).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_a_datetime = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_datetime_notnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "One" },
                    new QueryNullableItem { NullableDateTime = null, StringValue = "Null" },
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "Two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDateTime != null).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_a_datetime = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_datetime_not_hasvalue_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "One" },
                    new QueryNullableItem { NullableDateTime = null, StringValue = "Null" },
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "Two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDateTime.HasValue == false).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_an_nullableint_being_null =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_nullable_datetime_isnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "One" },
                    new QueryNullableItem { NullableDateTime = null, StringValue = "Null" },
					new QueryNullableItem { NullableDateTime = SysDateTime.Now, StringValue = "Two"}
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.NullableDateTime == null).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_an_nullabledatetime_being_null =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_string_notnull_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = 1, StringValue = "1" },
					new QueryNullableItem { NullableInt = 2, StringValue = null },
					new QueryNullableItem { NullableInt = 3, StringValue = "2" }
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.StringValue != null).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_structures_having_not_null_in_stringvalue = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_using_string_null_in_expression : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new List<QueryNullableItem>
				{
					new QueryNullableItem { NullableInt = 1, StringValue = "1" },
					new QueryNullableItem { NullableInt = 2, StringValue = null },
					new QueryNullableItem { NullableInt = 3, StringValue = "2" }
				};

                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNullableItem>()
                .Where(i => i.StringValue == null).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_structure_having_null_in_stringvalue = 
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryNullableItem> _structures;
            private static IList<QueryNullableItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_expression_does_not_match_any_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder < 0).ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json")]
        public class when_expression_does_not_match_any_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder < 0).ToListOfJson();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X")]
        public class when_expression_does_not_match_any_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder < 0).ToListOf<QueryItemInfo>();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder >= 2 && i.SortOrder <= 3).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder >= 2 && i.SortOrder <= 3).ToListOfJson();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder >= 2 && i.SortOrder <= 3).ToListOf<QueryItemInfo>();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_expression_contains_item_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json")]
        public class when_expression_contains_item_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder).ToListOfJson();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X")]
        public class when_expression_contains_item_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<QueryGuidItem>().Where(i => i.SortOrder >= _structures[1].SortOrder && i.SortOrder <= _structures[2].SortOrder).ToListOf<QueryItemInfo>();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_expression_contains_local_constants_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                const int @from = 2;
                const int @to = 3;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(i => i.SortOrder >= @from && i.SortOrder <= @to).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json")]
        public class when_expression_contains_local_constants_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                const int @from = 2;
                const int @to = 3;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(i => i.SortOrder >= @from && i.SortOrder <= @to).ToListOfJson();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X")]
        public class when_expression_contains_local_constants_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                const int @from = 2;
                const int @to = 3;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(i => i.SortOrder >= @from && i.SortOrder <= @to).ToListOf<QueryItemInfo>();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_expression_contains_variables_matching_two_middle_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var @from = 2;
                var @to = 3;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(i => i.SortOrder >= @from && i.SortOrder <= @to).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json")]
        public class when_expression_contains_variables_matching_two_middle_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var @from = 2;
                var @to = 3;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(i => i.SortOrder >= @from && i.SortOrder <= @to).ToListOfJson();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X")]
        public class when_expression_contains_variables_matching_two_middle_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var @from = 2;
                var @to = 3;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(i => i.SortOrder >= @from && i.SortOrder <= @to).ToListOf<QueryItemInfo>();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query (nested items)")]
        public class when_expression_contains_variables_matching_two_middle_nested_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryNestedGuidItem.CreateFourNestedItems();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var @from = 12;
                var @to = 13;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNestedGuidItem>().Where(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to).ToList();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryNestedGuidItem> _structures;
            private static IList<QueryNestedGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json (nested items)")]
        public class when_expression_contains_variables_matching_two_middle_nested_json_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryNestedGuidItem.CreateFourNestedItems();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var @from = 12;
                var @to = 13;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNestedGuidItem>().Where(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to).ToListOfJson();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryNestedGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X (nested items)")]
        public class when_expression_contains_variables_matching_two_middle_nested_structures_and_expects_X_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryNestedGuidItem.CreateFourNestedItems();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                var @from = 12;
                var @to = 13;
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryNestedGuidItem>().Where(i => i.Container.NestedInt >= @from && i.Container.NestedInt <= @to).ToListOf<QueryNestedItemInfo>();
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryNestedGuidItem> _structures;
            private static IList<QueryNestedItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Take")]
        public class when_query_matches_the_three_last_structures_of_four_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().Where(i => i.SortOrder >= 2 && i.SortOrder <= 4).Take(2).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json with Take")]
        public class when_query_matches_the_three_last_json_structures_of_four_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().Where(i => i.SortOrder >= 2 && i.SortOrder <= 4).Take(2).ToListOfJson();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X with Take")]
        public class when_query_matches_the_three_last_structures_of_four_and_take_is_two_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().Where(i => i.SortOrder >= 2 && i.SortOrder <= 4).Take(2).ToListOf<QueryItemInfo>();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Matches(_structures[1]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[2]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Take and Sort")]
        public class when_set_is_unsorted_and_query_matches_three_of_four_structures_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().Where(i => i.SortOrder == 2 || (i.SortOrder == 1 && i.StringValue == "B")).Take(2).OrderBy(i => i.StringValue).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_second_and_third_but_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[2]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json with Take and Sort")]
        public class when_set_is_unsorted_and_query_matches_three_of_four_json_structures_and_take_is_two : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().Where(i => i.SortOrder == 2 || (i.SortOrder == 1 && i.StringValue == "B")).Take(2).OrderBy(i => i.StringValue).ToListOfJson();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_second_and_third_but_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[2].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[1].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X with Take and Sort")]
        public class when_set_is_unsorted_and_query_matches_three_of_four_structures_and_take_is_two_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().Where(i => i.SortOrder == 2 || (i.SortOrder == 1 && i.StringValue == "B")).Take(2).OrderBy(i => i.StringValue, i => i.SortOrder).ToListOf<QueryItemInfo>();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_second_and_third_but_in_reverse = () =>
            {
                _fetchedStructures[0].Matches(_structures[2]).ShouldBeTrue();
                _fetchedStructures[1].Matches(_structures[1]).ShouldBeTrue();
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Skip and Sort")]
        public class when_query_skips_5_of_ten : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>()
                .OrderBy(i => i.SortOrder)
                .Skip(5).ToList();

            It should_have_fetched_five_structures =
                () => _fetchedStructures.Count.ShouldEqual(5);

            It should_have_fetched_the_five_last_structures =
                () => _fetchedStructures.ShouldBeValueEqualTo(_structures.Skip(5).ToArray());

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Skip, Take and Sort")]
        public class when_query_skips_5_of_ten_and_takes_2 : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>()
                .OrderBy(i => i.SortOrder)
                .Skip(5).Take(2).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_sixth_and_seventh_structures =
                () => _fetchedStructures.ShouldBeValueEqualTo(new []{_structures[5], _structures[6]});

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with First")]
        public class when_first_of_ten : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () 
                => _fetchedStructure = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().First();

            It should_have_fetched_the_first_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[0]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with First")]
        public class when_first_or_default_of_ten : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = ()
                => _fetchedStructure = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().FirstOrDefault();

            It should_have_fetched_the_first_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[0]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with First and Sort")]
        public class when_first_and_reverse_sort_of_ten : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = ()
                => _fetchedStructure = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().OrderByDescending(s => s.SortOrder).First();

            It should_have_fetched_the_last_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[9]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with First and Sort")]
        public class when_first_or_default_and_reverse_sort_of_ten : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = ()
                => _fetchedStructure = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().OrderByDescending(s => s.SortOrder).FirstOrDefault();

            It should_have_fetched_the_last_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[9]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Sort and Page")]
        public class when_query_skips_first_and_last_and_then_creates_pages_of_size_7_and_asks_for_the_last_page : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>()
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .OrderBy(i => i.SortOrder)
                .Page(1, 7).ToList();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[8]);

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Json Sort and Page")]
        public class when_query_skips_first_and_last_and_then_creates_pages_of_size_7_and_asks_for_the_last_page_as_Json : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>()
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .OrderBy(i => i.SortOrder)
                .Page(1, 7).ToListOfJson();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure =
                () => _fetchedStructures[0].ShouldEqual(_structures[8].AsJson());

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as X with Sort and Page")]
        public class when_query_skips_first_and_last_and_then_creates_pages_of_size_7_and_asks_for_the_last_page_as_X : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateTenItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>()
                .Where(i => i.SortOrder > 1 && i.SortOrder < 10)
                .OrderBy(i => i.SortOrder)
                .Page(1, 7).ToListOf<QueryItemInfo>();

            It should_have_fetched_one_structure =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_the_eight_structure =
                () => _fetchedStructures[0].Matches(_structures[8]).ShouldBeTrue();

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryItemInfo> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Anonymous array")]
        public class when_expression_does_not_match_any_structures_and_expects_array_of_anonymous_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>()
                .Where(i => i.SortOrder < 0)
                .ToArrayOf(new { IntegerValue = 0, StringValue = "" })
                .Select(i => new Tuple<int, string>(i.IntegerValue, i.StringValue))
                .ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<Tuple<int, string>> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Anonymous array")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures_and_expects_array_of_anonymous_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>()
                .Where(i => i.SortOrder >= 2 && i.SortOrder <= 3)
                .ToArrayOf(new { IntegerValue = 0, StringValue = "" })
                .Select(i => new Tuple<int, string>(i.IntegerValue, i.StringValue))
                .ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Item1.ShouldEqual(_structures[1].IntegerValue);
                _fetchedStructures[0].Item2.ShouldEqual(_structures[1].StringValue);

                _fetchedStructures[1].Item1.ShouldEqual(_structures[2].IntegerValue);
                _fetchedStructures[1].Item2.ShouldEqual(_structures[2].StringValue);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<Tuple<int, string>> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Anonymous list")]
        public class when_expression_does_not_match_any_structures_and_expects_list_of_anonymous_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(QueryGuidItem.CreateFourItems<QueryGuidItem>());
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>()
                .Where(i => i.SortOrder < 0)
                .ToListOf(new { IntegerValue = 0, StringValue = "" })
                .Select(i => new Tuple<int, string>(i.IntegerValue, i.StringValue))
                .ToList();

            It should_not_have_fetched_any_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<Tuple<int, string>> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query as Anonymous list")]
        public class when_providing_inline_constants_in_expression_matching_two_middle_structures_and_expects_lists_of_anonymous_as_result : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>()
                .Where(i => i.SortOrder >= 2 && i.SortOrder <= 3)
                .ToListOf(new { IntegerValue = 0, StringValue = "" })
                .Select(i => new Tuple<int, string>(i.IntegerValue, i.StringValue))
                .ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].Item1.ShouldEqual(_structures[1].IntegerValue);
                _fetchedStructures[0].Item2.ShouldEqual(_structures[1].StringValue);

                _fetchedStructures[1].Item1.ShouldEqual(_structures[2].IntegerValue);
                _fetchedStructures[1].Item2.ShouldEqual(_structures[2].StringValue);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<Tuple<int, string>> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query")]
        public class when_query_matches_two_of_four_items_that_are_in_uncommitted_mode : SpecificationBase
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

                    _fetchedStructures = session.Query<QueryGuidItem>().Where(x => x.SortOrder >= _structures[1].SortOrder && x.SortOrder <= _structures[2].SortOrder).ToList();
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

        [Subject(typeof(ISisoQueryable<>), "Query with Sort")]
        public class when_sorting_on_nullable_int_that_has_value: SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().OrderBy(i => i.NullableIntegerValue).ToList();

            It should_have_fetched_all_four_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_have_fetched_all_structures_but_sorted_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[3]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
                _fetchedStructures[2].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[3].ShouldBeValueEqualTo(_structures[0]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Sort")]
        public class when_sorting_on_guid : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().OrderBy(i => i.GuidValue).ToList();

            It should_have_fetched_all_four_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_have_fetched_all_structures_but_sorted_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[3]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
                _fetchedStructures[2].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[3].ShouldBeValueEqualTo(_structures[0]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "Query with Sort")]
        public class when_sorting_on_bool : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourUnorderedItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .Query<QueryGuidItem>().OrderBy(i => i.BoolValue, i => i.IntegerValue).ToList();

            It should_have_fetched_all_four_structures =
                () => _fetchedStructures.Count.ShouldEqual(4);

            It should_have_fetched_all_structures_but_sorted_in_reverse = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[3]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
                _fetchedStructures[2].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[3].ShouldBeValueEqualTo(_structures[0]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof (ISession), "Query using string lambda")]
        public class when_string_expression_matches_two_middle_structures : SpecificationBase
        {
            private Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            private Because of = () =>
            {
                var from = _structures[1].SortOrder;
                var to = _structures[2].SortOrder;

                var builder = new DynamicLambdaBuilder();
                var expression = builder.BuildPredicate<QueryGuidItem>("i => i.SortOrder >= {0} && i.SortOrder <= {1}", from, to);
                _fetchedStructures = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().Where(expression).ToList();
            };
        

        It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }
    }
}