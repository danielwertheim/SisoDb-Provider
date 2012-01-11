using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PineCone;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.UnitOfWork
{
	class InsertsComplete
    {
        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_complete_guid_entity_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = ModelFactory.CreateItems<CompleteGuidEntity, Guid>(1).Single();
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(new[] { _structure });

            It should_have_been_inserted =
                () => TestContext.Database.should_have_X_num_of_items<CompleteGuidEntity>(1);

            It should_have_been_inserted_completely =
                () => TestContext.Database.should_have_identical_structures(_structure);

            private static CompleteGuidEntity _structure;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_two_complete_guid_entities_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = ModelFactory.CreateItems<CompleteGuidEntity, Guid>(2);
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(_structures);

            It should_have_inserted_both =
                () => TestContext.Database.should_have_X_num_of_items<CompleteGuidEntity>(2);

            It should_have_inserted_both_completely =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<CompleteGuidEntity> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_complete_string_entity_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = ModelFactory.CreateItems<CompleteStringEntity, string>(1).Single();
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(new[] { _structure });

            It should_have_been_inserted =
                () => TestContext.Database.should_have_X_num_of_items<CompleteStringEntity>(1);

            It should_have_been_inserted_completely =
                () => TestContext.Database.should_have_identical_structures(_structure);

            private static CompleteStringEntity _structure;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_two_complete_string_entities_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = ModelFactory.CreateItems<CompleteStringEntity, string>(2);
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(_structures);

            It should_have_inserted_both =
                () => TestContext.Database.should_have_X_num_of_items<CompleteStringEntity>(2);

            It should_have_inserted_both_completely =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<CompleteStringEntity> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_complete_identity_entity_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = ModelFactory.CreateItems<CompleteIdentityEntity, int>(1).Single();
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(new[] { _structure });

            It should_have_been_inserted =
                () => TestContext.Database.should_have_X_num_of_items<CompleteIdentityEntity>(1);

            It should_have_been_inserted_completely =
                () => TestContext.Database.should_have_identical_structures(_structure);

            private static CompleteIdentityEntity _structure;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_two_complete_identity_entities_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = ModelFactory.CreateItems<CompleteIdentityEntity, int>(2);
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(_structures);

            It should_have_inserted_both =
                () => TestContext.Database.should_have_X_num_of_items<CompleteIdentityEntity>(2);

            It should_have_inserted_both_completely =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<CompleteIdentityEntity> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_complete_big_identity_entity_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = ModelFactory.CreateItems<CompleteBigIdentityEntity, long>(1).Single();
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(new[] { _structure });

            It should_have_been_inserted =
                () => TestContext.Database.should_have_X_num_of_items<CompleteBigIdentityEntity>(1);

            It should_have_been_inserted_completely =
                () => TestContext.Database.should_have_identical_structures(_structure);

            private static CompleteBigIdentityEntity _structure;
        }

        [Subject(typeof(IUnitOfWork), "Insert (complete)")]
        public class when_inserting_two_complete_big_identity_entities_with_populated_hierarchy : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = ModelFactory.CreateItems<CompleteBigIdentityEntity, long>(2);
            };

            Because of =
                () => TestContext.Database.WriteOnce().InsertMany(_structures);

            It should_have_inserted_both =
                () => TestContext.Database.should_have_X_num_of_items<CompleteBigIdentityEntity>(2);

            It should_have_inserted_both_completely =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<CompleteBigIdentityEntity> _structures;
        }

        public static class ModelFactory
        {
            public static IList<T> CreateItems<T, TId>(int numOfItems) where T : Entity<TId>, new()
            {
                var items = new List<T>(numOfItems);

                for (var c = 0; c < numOfItems; c++)
                {
                    var item = new T
                    {
                        Names = new Names("Daniel", "Wertheim"),
                        NullValuesContainer = new NullValuesContainer
                        {
                            Bool = true,
                            Decimal = 3.14m,
                            Double = 1.33,
                            Guid = Guid.Parse("60D977F9-95FC-40FD-9A7E-6827E920370F"),
                            Int = 42,
                            Long = 142,
                            String = "String in NullValuesContainer.",
							Text= "Some text."
                        },
                        NullValuesContainerWithNulls = new NullValuesContainer(),
                        Values = new ValuesContainer
                        {
                            Value = new Value { Is = 99 },
                            Bool = true,
                            Decimal = 3.14m,
                            Double = 1.33,
                            Guid = Guid.Parse("19ADFA6D-A127-48E0-9291-AEB75E8CA23C"),
                            Int = 42,
                            Long = 142,
                            String = "String in ValuesContainer.",
							Text = "Some text."
                        },
                        ValuesInArray = new[] { new Value { Is = 1 }, new Value { Is = 2 }, new Value { Is = 3 } },
                        //ValuesInISet = new HashSet<Value> { new Value { Is = 11 }, new Value { Is = 12 }, new Value { Is = 13 } },
                        ValuesInHashSet = new HashSet<Value> { new Value { Is = 21 }, new Value { Is = 22 }, new Value { Is = 23 } },
                        ValuesInIList = new List<Value> { new Value { Is = 31 }, new Value { Is = 32 }, new Value { Is = 33 } },
                        ValuesInList = new List<Value> { new Value { Is = 41 }, new Value { Is = 42 }, new Value { Is = 43 } },
                        KeyValuesInDictionary = new Dictionary<string, int> { { "A", 44 }, { "B", 45 }, { "C", 46 } },
                        KeyValuesInIDictionary = new Dictionary<string, int> { { "A", 44 }, { "B", 45 }, { "C", 46 } },
                        ValuesInIDictionary = new Dictionary<int, Value>
                        {
                            { 51, new Value { Is = 51 } }, { 52, new Value { Is = 52 } }, { 53, new Value { Is = 53 } }
                        },
                        ValuesInDictionary = new Dictionary<int, Value>
                        {
                            { 61, new Value { Is = 61 } }, { 62, new Value { Is = 62 } }, { 63, new Value { Is = 63 } }
                        }
                    };

                    if (item is Entity<string>)
                        (item as Entity<string>).StructureId = (c + 1).ToString();

                    items.Add(item);
                }

                return items;
            }
        }

        [Serializable]
        public class CompleteStringEntity : Entity<string>
        { }

        [Serializable]
        public class CompleteGuidEntity : Entity<Guid>
        { }

        [Serializable]
        public class CompleteIdentityEntity : Entity<int>
        { }

        [Serializable]
        public class CompleteBigIdentityEntity : Entity<long>
        { }

        [Serializable]
        public abstract class Entity<T>
        {
            public T StructureId { get; set; }

            public Names Names { get; set; }

            public ValuesContainer Values { get; set; }

            public NullValuesContainer NullValuesContainer { get; set; }

            public NullValuesContainer NullValuesContainerWithNulls { get; set; }

            public Value[] ValuesInArray { get; set; }

            public IList<Value> ValuesInIList { get; set; }

            public List<Value> ValuesInList { get; set; }

            //public ISet<Value> ValuesInISet { get; set; }

            public HashSet<Value> ValuesInHashSet { get; set; }

            public IDictionary<string, int> KeyValuesInIDictionary { get; set; }

            public Dictionary<string, int> KeyValuesInDictionary { get; set; }

            public IDictionary<int, Value> ValuesInIDictionary { get; set; }

            public Dictionary<int, Value> ValuesInDictionary { get; set; }
        }

        [Serializable]
        public class Value
        {
            public int Is { get; set; }
        }

        [Serializable]
        public class ValuesContainer
        {
            public Value Value { get; set; }

            public Guid Guid { get; set; }

            public int Int { get; set; }

            public long Long { get; set; }

            public string String { get; set; }

			public Text Text { get; set; }

            public double Double { get; set; }

            public decimal Decimal { get; set; }

            public bool Bool { get; set; }
        }

        [Serializable]
        public class NullValuesContainer
        {
            public Guid? Guid { get; set; }

            public int? Int { get; set; }

            public long? Long { get; set; }

            public string String { get; set; }

			public Text Text { get; set; }

            public double? Double { get; set; }

            public decimal? Decimal { get; set; }

            public bool? Bool { get; set; }
        }

        [Serializable]
        public class Names : IEquatable<Names>
        {
            public Names(string first, string last)
            {
                First = first;
                Last = last;
            }

            public string First { get; private set; }

            public string Last { get; private set; }

            public bool Equals(Names other)
            {
                return Equals(other.First, First) && Equals(other.Last, Last);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof(Names)) return false;
                return Equals((Names)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((First != null ? First.GetHashCode() : 0) * 397) ^ (Last != null ? Last.GetHashCode() : 0);
                }
            }
        }
    }
}