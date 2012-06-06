using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.EnumerableFunctions
{
    class QxIn
    {
        [Subject(typeof(ISession), "QxIn")]
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

            It should_have_returned_the_two_matching_structures = () =>
            {
                _refetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _refetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<Model> _structures;
            private static IList<Model> _refetchedStructures;
        }

        private class Model
        {
            public Guid Id { get; set; }
            public int IntValue { get; set; }
            public int[] IntValues { get; set; }
            public Bag Bag { get; set; }
            public Bag[] Bags { get; set; }
        }

        private class Bag
        {
            public int[] IntValues { get; set; }
        }
    }
}