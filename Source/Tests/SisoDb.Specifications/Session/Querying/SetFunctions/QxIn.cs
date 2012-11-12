using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.SetFunctions
{
    namespace QxIn
    {
        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_strings_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { StringValue = "A" }, 
                    new Model { StringValue = "B" }, 
                    new Model { StringValue = "C" }, 
                    new Model { StringValue = "D" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.StringValue.QxIn("B", "C")).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_array_of_strings_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { StringValues = new[] { "A", "B" } }, 
                    new Model { StringValues = new[] { "C", "D" } }, 
                    new Model { StringValues = new[] { "C", "D" } }, 
                    new Model { StringValues = new[] { "E", "F" } }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.StringValues.QxIn("C", "D")).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_dates_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { DateTimeValue = TestConstants.FixedDateTime.AddDays(1) }, 
                    new Model { DateTimeValue = TestConstants.FixedDateTime.AddDays(2) }, 
                    new Model { DateTimeValue = TestConstants.FixedDateTime.AddDays(3) }, 
                    new Model { DateTimeValue = TestConstants.FixedDateTime.AddDays(4) }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.DateTimeValue.QxIn(TestConstants.FixedDateTime.AddDays(2), TestConstants.FixedDateTime.AddDays(3))).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_array_of_dates_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { DateTimeValues = new [] { TestConstants.FixedDateTime.AddDays(1), TestConstants.FixedDateTime.AddDays(2) } }, 
                    new Model { DateTimeValues = new [] { TestConstants.FixedDateTime.AddDays(3), TestConstants.FixedDateTime.AddDays(4) } }, 
                    new Model { DateTimeValues = new [] { TestConstants.FixedDateTime.AddDays(3), TestConstants.FixedDateTime.AddDays(4) } }, 
                    new Model { DateTimeValues = new [] { TestConstants.FixedDateTime.AddDays(5), TestConstants.FixedDateTime.AddDays(6) } }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.DateTimeValues.QxIn(TestConstants.FixedDateTime.AddDays(3), TestConstants.FixedDateTime.AddDays(4))).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { IntValue = 1 }, 
                    new Model { IntValue = 2 }, 
                    new Model { IntValue = 3 }, 
                    new Model { IntValue = 4 }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.IntValue.QxIn(2, 3)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nullable_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { NullableIntValue = 1 }, 
                    new Model { NullableIntValue = 2 }, 
                    new Model { NullableIntValue = 3 }, 
                    new Model { NullableIntValue = 4 }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.NullableIntValue.QxIn(2, 3)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_array_of_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { IntValues = new []{ 1, 2 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 5, 6 } }, 
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.IntValues.QxIn(3, 4)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_array_of_nullable_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { NullableIntValues = new int?[]{ 1, 2 } }, 
                    new Model { NullableIntValues = new int?[]{ 3, 4 } }, 
                    new Model { NullableIntValues = new int?[]{ 3, 4 } }, 
                    new Model { NullableIntValues = new int?[]{ 5, 6 } }, 
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.NullableIntValues.QxIn(3, 4)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nested_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { Bag = new Bag{IntValue = 1} }, 
                    new Model { Bag = new Bag{IntValue = 2} },
                    new Model { Bag = new Bag{IntValue = 3} },
                    new Model { Bag = new Bag{IntValue = 4} },
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.Bag.IntValue.QxIn(2, 3)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nested_nullable_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { Bag = new Bag{NullableIntValue = 1} }, 
                    new Model { Bag = new Bag{NullableIntValue = 2} },
                    new Model { Bag = new Bag{NullableIntValue = 3} },
                    new Model { Bag = new Bag{NullableIntValue = 4} }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.Bag.NullableIntValue.QxIn(2, 3)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nested_array_of_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { Bag = new Bag{IntValues = new []{1,2}} }, 
                    new Model { Bag = new Bag{IntValues = new []{3,4}} }, 
                    new Model { Bag = new Bag{IntValues = new []{3,4}} }, 
                    new Model { Bag = new Bag{IntValues = new []{5,6}} }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.Bag.IntValues.QxIn(3, 4)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nested_array_of_nullable_integers_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { Bag = new Bag{NullableIntValues = new int?[]{1,2}} }, 
                    new Model { Bag = new Bag{NullableIntValues = new int?[]{3,4}} }, 
                    new Model { Bag = new Bag{NullableIntValues = new int?[]{3,4}} }, 
                    new Model { Bag = new Bag{NullableIntValues = new int?[]{5,6}} }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.Bag.NullableIntValues.QxIn(3, 4)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nested_integers_on_array_of_complex_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { Bags = new [] { new Bag { IntValue = 1 }, new Bag { IntValue = 2 } } }, 
                    new Model { Bags = new [] { new Bag { IntValue = 3 }, new Bag { IntValue = 4 } } }, 
                    new Model { Bags = new [] { new Bag { IntValue = 3 }, new Bag { IntValue = 4 } } }, 
                    new Model { Bags = new [] { new Bag { IntValue = 5 }, new Bag { IntValue = 6 } } }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.Bags.QxAny(b => b.IntValue.QxIn(3,4))).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxIn")]
        public class when_querying_nested_array_of_integers_on_array_of_complex_and_two_of_four_in_set_matches_two_of_four_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { Bags = new [] { new Bag { IntValues = new [] { 1, 2 } }, new Bag { IntValues = new [] { 1, 2 } } } }, 
                    new Model { Bags = new [] { new Bag { IntValues = new [] { 3, 4 } }, new Bag { IntValues = new [] { 3, 4 } } } }, 
                    new Model { Bags = new [] { new Bag { IntValues = new [] { 3, 4 } }, new Bag { IntValues = new [] { 3, 4 } } } }, 
                    new Model { Bags = new [] { new Bag { IntValues = new [] { 5, 6 } }, new Bag { IntValues = new [] { 5, 6 } } } }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.Bags.QxAny(b => b.IntValues.QxIn(3, 4))).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "!QxIn")]
        public class when_negated_query_of_integer_array_matches_two_of_four_in_set_of_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { IntValues = new []{ 1, 2 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 5, 6 } }, 
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => !m.IntValues.QxIn(3, 4)).ToList();
            };

            It should_have_returned_only_two_structures = () =>
                _refetchedStructures.Count.ShouldEqual(2);

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "!QxIn")]
        public class when_negated_query_of_integer_array_matches_all_in_set_of_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { IntValues = new []{ 1, 2 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 5, 6 } }, 
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => !m.IntValues.QxIn(0, 7)).ToList();
            };

            It should_have_returned_all_four_structures = () =>
                _refetchedStructures.Count.ShouldEqual(4);

            It should_have_returned_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[2].ShouldBeValueEqualTo(_structures[2]);
                _refetchedStructures[3].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        [Subject(typeof(ISisoQueryable<>), "QxNotIn")]
        public class when_query_of_integer_array_matches_all_in_set_of_structures : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { IntValues = new []{ 1, 2 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 3, 4 } }, 
                    new Model { IntValues = new []{ 5, 6 } }, 
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                _refetchedStructures = TestContext.Database.UseOnceTo()
                    .Query<Model>()
                    .Where(m => m.IntValues.QxNotIn(0, 7)).ToList();
            };

            It should_have_returned_all_four_structures = () =>
                _refetchedStructures.Count.ShouldEqual(4);

            It should_have_returned_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[2].ShouldBeValueEqualTo(_structures[2]);
                _refetchedStructures[3].ShouldBeValueEqualTo(_structures[3]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        public class Model
        {
            public Guid Id { get; set; }
            public int IntValue { get; set; }
            public int? NullableIntValue { get; set; }
            public int[] IntValues { get; set; }
            public int?[] NullableIntValues { get; set; }
            public string StringValue { get; set; }
            public string[] StringValues { get; set; }
            public DateTime? DateTimeValue { get; set; }
            public DateTime[] DateTimeValues { get; set; }
            public Bag Bag { get; set; }
            public Bag[] Bags { get; set; }
        }

        public class Bag
        {
            public int IntValue { get; set; }
            public int? NullableIntValue { get; set; }
            public int[] IntValues { get; set; }
            public int?[] NullableIntValues { get; set; }
        }
    }
}