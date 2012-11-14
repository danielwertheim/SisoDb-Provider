using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.NCore.Reflections;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Session
{
	class Updates
    {
        [Subject(typeof(ISession), "Update")]
        public class when_guiditem_and_updating_two_of_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertGuidItems(4);
                _orgItem1 = CopyObject.Deep(_structures[1]);
                _orgItem2 = CopyObject.Deep(_structures[2]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[1].Value += 10;
                    session.Update(_structures[1]);

                    _structures[2].Value += 10;
                    session.Update(_structures[2]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[1].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[2].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<GuidItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<GuidItem> _structures;
            private static GuidItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update")]
        public class when_stringitem_and_updating_two_of_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertStringItems(4);
                _orgItem1 = CopyObject.Deep(_structures[1]);
                _orgItem2 = CopyObject.Deep(_structures[2]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[1].Value += 10;
                    session.Update(_structures[1]);

                    _structures[2].Value += 10;
                    session.Update(_structures[2]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<StringItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[1].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[2].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<StringItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<StringItem> _structures;
            private static StringItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update")]
        public class when_identityitem_and_updating_two_of_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertIdentityItems(4);
                _orgItem1 = CopyObject.Deep(_structures[1]);
                _orgItem2 = CopyObject.Deep(_structures[2]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[1].Value += 10;
                    session.Update(_structures[1]);

                    _structures[2].Value += 10;
                    session.Update(_structures[2]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[1].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[2].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<IdentityItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<IdentityItem> _structures;
            private static IdentityItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update")]
        public class when_bigidentityitem_and_updating_two_of_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _orgItem1 = CopyObject.Deep(_structures[1]);
                _orgItem2 = CopyObject.Deep(_structures[2]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[1].Value += 10;
                    session.Update(_structures[1]);

                    _structures[2].Value += 10;
                    session.Update(_structures[2]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[1].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[2].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<BigIdentityItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<BigIdentityItem> _structures;
            private static BigIdentityItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update (uniques)")]
        public class when_uniqueguiditem_and_updating_from_unique_to_non_unique : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertUniqueGuidItems(2);
                _orgItem1 = CopyObject.Deep(_structures[0]);
                _orgItem2 = CopyObject.Deep(_structures[1]);
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _structures[0].UniqueValue = _structures[1].UniqueValue;
                        session.Update(_structures[0]);
                    }
                });
            };

            It should_fail =
                () => CaughtException.ShouldNotBeNull();

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<UniqueGuidItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[0].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[1].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<UniqueGuidItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_not_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_orgItem1, _orgItem2);

            private static IList<UniqueGuidItem> _structures;
            private static UniqueGuidItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update (uniques)")]
        public class when_uniqueidentityitem_and_updating_from_unique_to_non_unique : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertUniqueIdentityItems(2);
                _orgItem1 = CopyObject.Deep(_structures[0]);
                _orgItem2 = CopyObject.Deep(_structures[1]);
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _structures[0].UniqueValue = _structures[1].UniqueValue;
                        session.Update(_structures[0]);
                    }
                });
            };

            It should_fail =
                () => CaughtException.ShouldNotBeNull();

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<UniqueIdentityItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[0].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[1].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<UniqueIdentityItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_not_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_orgItem1, _orgItem2);

            private static IList<UniqueIdentityItem> _structures;
            private static UniqueIdentityItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update (uniques)")]
        public class when_uniquebigidentityitem_and_updating_from_unique_to_non_unique : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertUniqueBigIdentityItems(2);
                _orgItem1 = CopyObject.Deep(_structures[0]);
                _orgItem2 = CopyObject.Deep(_structures[1]);
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _structures[0].UniqueValue = _structures[1].UniqueValue;
                        session.Update(_structures[0]);
                    }
                });
            };

            It should_fail =
                () => CaughtException.ShouldNotBeNull();

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<UniqueBigIdentityItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[0].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[1].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<UniqueBigIdentityItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_not_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_orgItem1, _orgItem2);

            private static IList<UniqueBigIdentityItem> _structures;
            private static UniqueBigIdentityItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update (uniques)")]
        public class when_uniqueguiditem_and_updating_from_unique_to_other_unique : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertUniqueGuidItems(2);
                _orgItem1 = CopyObject.Deep(_structures[0]);
                _orgItem2 = CopyObject.Deep(_structures[1]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[0].UniqueValue = 42;
                    session.Update(_structures[0]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<UniqueGuidItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[0].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[1].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<UniqueGuidItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<UniqueGuidItem> _structures;
            private static UniqueGuidItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update (uniques)")]
        public class when_uniqueidentityitem_and_updating_from_unique_to_other_unique : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertUniqueIdentityItems(2);
                _orgItem1 = CopyObject.Deep(_structures[0]);
                _orgItem2 = CopyObject.Deep(_structures[1]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[0].UniqueValue = 42;
                    session.Update(_structures[0]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<UniqueIdentityItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[0].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[1].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<UniqueIdentityItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<UniqueIdentityItem> _structures;
            private static UniqueIdentityItem _orgItem1, _orgItem2;
        }

        [Subject(typeof(ISession), "Update (uniques)")]
        public class when_uniquebigidentityitem_and_updating_from_unique_to_other_unique : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = TestContext.Database.InsertUniqueBigIdentityItems(2);
                _orgItem1 = CopyObject.Deep(_structures[0]);
                _orgItem2 = CopyObject.Deep(_structures[1]);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _structures[0].UniqueValue = 42;
                    session.Update(_structures[0]);
                }
            };

            It should_have_the_same_number_of_structures_in_database =
                () => TestContext.Database.should_have_X_num_of_items<UniqueBigIdentityItem>(_structures.Count);

            It should_not_have_changed_ids_of_the_structures_in_memory = () =>
            {
                _structures[0].StructureId.ShouldEqual(_orgItem1.StructureId);
                _structures[1].StructureId.ShouldEqual(_orgItem2.StructureId);
            };

            It should_not_have_changed_ids_of_the_structures_in_database =
                () => TestContext.Database.should_have_ids<UniqueBigIdentityItem>(_orgItem1.StructureId, _orgItem2.StructureId);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(_structures.ToArray());

            private static IList<UniqueBigIdentityItem> _structures;
            private static UniqueBigIdentityItem _orgItem1, _orgItem2;
        }
    }
}