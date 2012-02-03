using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine.StringFunctions
{
	class QxEndsWith
    {
        [Subject(typeof(IReadSession), "QxEndsWith")]
        public class when_two_items_has_string_that_does_not_match_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.WriteOnce().InsertMany(QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                }));
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryGuidItem>().Where(i => i.StringValue.QxEndsWith("Foo")).ToList();

            It should_not_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IReadSession), "QxEndsWith")]
        public class when_two_items_has_string_that_ends_with_queried_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                });
                TestContext.Database.WriteOnce().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryGuidItem>().Where(i => i.StringValue.QxEndsWith("ha")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(IReadSession), "QxEndsWith")]
        public class when_two_items_has_string_that_completely_matches_argument : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateItems<QueryGuidItem>(10, (i, item) =>
                {
                    item.SortOrder = i + 1;
                    item.StringValue = item.SortOrder <= 2 ? "Alpha" : "Bravo";
                });
                TestContext.Database.WriteOnce().InsertMany(_structures);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryGuidItem>().Where(i => i.StringValue.QxEndsWith("Alpha")).ToList();

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_first_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

		[Subject(typeof(IReadSession), "QxEndsWith")]
		public class when_two_first_items_has_string_that_ends_with_queried_argument_on_tostring_of_int : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = new[]
				{
					new QueryGuidItem{IntegerValue = 141},
 					new QueryGuidItem{IntegerValue = 241},
					new QueryGuidItem{IntegerValue = 142},
					new QueryGuidItem{IntegerValue = 242}
				};
                TestContext.Database.WriteOnce().InsertMany(_structures);
			};

			Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryGuidItem>().Where(i => i.IntegerValue.ToString().QxEndsWith("41")).ToList();

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_two_first_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<QueryGuidItem> _fetchedStructures;
		}

		[Subject(typeof(IReadSession), "QxEndsWith")]
		public class when_two_last_items_has_string_that_ends_with_queried_argument_on_tostring_of_int : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = new[]
				{
					new QueryGuidItem{IntegerValue = 141},
 					new QueryGuidItem{IntegerValue = 241},
					new QueryGuidItem{IntegerValue = 142},
					new QueryGuidItem{IntegerValue = 242}
				};
                TestContext.Database.WriteOnce().InsertMany(_structures);
			};

			Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryGuidItem>().Where(i => i.IntegerValue.ToString().QxEndsWith("42")).ToList();

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_two_last_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[2]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<QueryGuidItem> _fetchedStructures;
		}

		[Subject(typeof(IReadSession), "QxEndsWith")]
		public class when_two_first_items_has_string_that_ends_with_queried_argument_on_tostring_of_nullable_int : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = new[]
				{
					new QueryNullableItem{NullableInt = 141},
 					new QueryNullableItem{NullableInt = 241},
					new QueryNullableItem{NullableInt = 142},
					new QueryNullableItem{NullableInt = 242}
				};
                TestContext.Database.WriteOnce().InsertMany(_structures);
			};

			Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryNullableItem>().Where(i => i.NullableInt.ToString().QxEndsWith("41")).ToList();

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_two_first_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
			};

			private static IList<QueryNullableItem> _structures;
			private static IList<QueryNullableItem> _fetchedStructures;
		}

		[Subject(typeof(IReadSession), "QxEndsWith")]
		public class when_two_last_items_has_string_that_ends_with_queried_argument_on_tostring_of_nullable_int_value : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = new[]
				{
					new QueryNullableItem{NullableInt = 141},
 					new QueryNullableItem{NullableInt = 241},
					new QueryNullableItem{NullableInt = 142},
					new QueryNullableItem{NullableInt = 242}
				};
                TestContext.Database.WriteOnce().InsertMany(_structures);
			};

			Because of =
				() => _fetchedStructures = TestContext.Database.ReadOnce()
					.Query<QueryNullableItem>().Where(i => i.NullableInt.Value.ToString().QxEndsWith("42")).ToList();

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_two_last_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[2]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
			};

			private static IList<QueryNullableItem> _structures;
			private static IList<QueryNullableItem> _fetchedStructures;
		}
    }
}