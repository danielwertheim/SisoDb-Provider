using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.PineCone.Annotations;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.Testing.Steps;
using SisoDb.DbSchema;

namespace SisoDb.Specifications.Session
{
    class InsertsOfUniquesPerType
    {
        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_guid_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structure = new VehicleWithGuidId { VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithGuidId>(1);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures(_structure);

            private static VehicleWithGuidId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_guid_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structures = new[]
                {
                    new VehicleWithGuidId { VehRegNo = "ABC123" },
                    new VehicleWithGuidId { VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_inserted_vehicles = () => TestContext.Database.should_have_X_num_of_items<VehicleWithGuidId>(2);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures<VehicleWithGuidId>(_structures.ToArray());

            private static IList<VehicleWithGuidId> _structures;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_non_unique_per_type_guid_entities : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgStructure = new VehicleWithGuidId { VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_orgStructure);
            };

            Because of =
                () => CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Insert(new VehicleWithGuidId { VehRegNo = "ABC123" }));


            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithGuidIdUniques' with unique index 'UQ_VehicleWithGuidIdUniques'.");
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithGuidId>(1);

            It should_have_inserted_first = () => TestContext.Database.should_have_identical_structures<VehicleWithGuidId>(new[] { _orgStructure });

            private static VehicleWithGuidId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_string_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structure = new VehicleWithStringId { StructureId = "A123", VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithStringId>(1);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures(_structure);

            private static VehicleWithStringId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_string_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structures = new[]
                {
                    new VehicleWithStringId { StructureId = "A1", VehRegNo = "ABC123" },
                    new VehicleWithStringId { StructureId = "A2", VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_inserted_vehicles = () => TestContext.Database.should_have_X_num_of_items<VehicleWithStringId>(2);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures<VehicleWithStringId>(_structures.ToArray());

            private static IList<VehicleWithStringId> _structures;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_non_unique_per_type_string_entities : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgStructure = new VehicleWithStringId { StructureId = "A1", VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_orgStructure);
            };

            Because of =
                () => CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Insert(new VehicleWithStringId { StructureId = "A2", VehRegNo = "ABC123" }));


            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithStringIdUniques' with unique index 'UQ_VehicleWithStringIdUniques'.");
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithStringId>(1);

            It should_have_inserted_first = () => TestContext.Database.should_have_identical_structures<VehicleWithStringId>(new[] { _orgStructure });

            private static VehicleWithStringId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_identity_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structure = new VehicleWithIdentityId { VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithIdentityId>(1);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures(_structure);

            private static VehicleWithIdentityId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_identity_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structures = new[]
                {
                    new VehicleWithIdentityId { VehRegNo = "ABC123" },
                    new VehicleWithIdentityId { VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_inserted_vehicles = () => TestContext.Database.should_have_X_num_of_items<VehicleWithIdentityId>(2);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures<VehicleWithIdentityId>(_structures.ToArray());

            private static IList<VehicleWithIdentityId> _structures;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_non_unique_per_type_identity_entities : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgStructure = new VehicleWithIdentityId {VehRegNo = "ABC123"};
                TestContext.Database.UseOnceTo().Insert(_orgStructure);
            };

            Because of =
                () => CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Insert(new VehicleWithIdentityId { VehRegNo = "ABC123" }));


            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithIdentityIdUniques' with unique index 'UQ_VehicleWithIdentityIdUniques'.");
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithIdentityId>(1);

            It should_have_inserted_first = () => TestContext.Database.should_have_identical_structures<VehicleWithIdentityId>(new[] { _orgStructure });

            private static VehicleWithIdentityId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_big_identity_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structure = new VehicleWithBigIdentityId {VehRegNo = "ABC123"};
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithBigIdentityId>(1);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures(_structure);

            private static VehicleWithBigIdentityId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_big_identity_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structures = new[]
                {
                    new VehicleWithBigIdentityId { VehRegNo = "ABC123" },
                    new VehicleWithBigIdentityId { VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_inserted_vehicles = () => TestContext.Database.should_have_X_num_of_items<VehicleWithBigIdentityId>(2);

            It should_get_inserted = () => TestContext.Database.should_have_identical_structures<VehicleWithBigIdentityId>(_structures.ToArray());

            private static IList<VehicleWithBigIdentityId> _structures;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_non_unique_per_type_big_identity_entities : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _orgStructure = new VehicleWithBigIdentityId {VehRegNo = "ABC123"};
                TestContext.Database.UseOnceTo().Insert(_orgStructure);
            };

            Because of =
                () => CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Insert(new VehicleWithBigIdentityId { VehRegNo = "ABC123" }));


            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithBigIdentityIdUniques' with unique index 'UQ_VehicleWithBigIdentityIdUniques'.");
            };

            It should_have_one_inserted_vehicle = () => TestContext.Database.should_have_X_num_of_items<VehicleWithBigIdentityId>(1);

            It should_have_inserted_first = () => TestContext.Database.should_have_identical_structures<VehicleWithBigIdentityId>(new[] { _orgStructure });

            private static VehicleWithBigIdentityId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_member_with_unique_constraint_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgStructure = new VehicleWithGuidId { VehRegNo = null };
            };

            Because of = () => CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Insert(_orgStructure));
            
            It should_have_failed = () =>
            {
                CaughtException.ShouldNotBeNull();
                (CaughtException as AggregateException).InnerExceptions[0].Message.ShouldStartWith("The Unique index 'VehicleWithGuidId':'VehRegNo' is evaluated to Null. This is not alowed.");
            };

            private static VehicleWithGuidId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_null_collection_with_items_having_member_with_unique_constraint : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<WithCollectionOfUnqies>();
                _orgStructure = new WithCollectionOfUnqies { Items = null };
            };

            Because of = () => TestContext.Database.UseOnceTo().Insert(_orgStructure);

            It should_have_stored_structure_with_null_values = () => TestContext.Database.should_have_identical_structures(new[] { _orgStructure });

            It should_not_have_stored_unique_member_wiht_null_value_in_uniques_table = () => 
                TestContext.DbHelper.RowCount(_structureSchema.GetUniquesTableName()).ShouldEqual(0);
            
            private static WithCollectionOfUnqies _orgStructure;
            private static IStructureSchema _structureSchema;
        }

        public class WithCollectionOfUnqies
        {
            public Guid StructureId { get; set; }
            public List<WithUniqueStringPerType> Items { get; set; }
        }

        public class WithUniqueStringPerType
        {
            [Unique(UniqueModes.PerType)]
            public string Key { get; set; }
        }

        public class VehicleWithGuidId : VehicleBase<Guid>
        {
        }

        public class VehicleWithStringId : VehicleBase<string>
        {
        }

        public class VehicleWithIdentityId : VehicleBase<int>
        {
        }

        public class VehicleWithBigIdentityId : VehicleBase<long>
        {
        }

        public class VehicleBase<TId>
        {
            public TId StructureId { get; set; }

            [Unique(UniqueModes.PerType)]
            public string VehRegNo { get; set; }
        }
    }
}