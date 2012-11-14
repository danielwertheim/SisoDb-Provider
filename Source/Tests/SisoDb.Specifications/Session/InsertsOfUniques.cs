using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;
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
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithGuidId>();
                _structure = new VehicleWithGuidId { VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithGuidId>(1);

            It should_have_inserted_structure_correctly = () => TestContext.Database.should_have_identical_structures(_structure);

            It should_have_inserted_value_in_unique_table = () => TestContext.DbHelper
                .UniquesTableHasMember<VehicleWithGuidId>(_structureSchema, _structure.StructureId, s => s.VehRegNo);

            It should_have_inserted_value_in_indexes_table = () => TestContext.DbHelper
                .should_have_stored_member_in_indexes_table<VehicleWithGuidId>(_structureSchema, _structure.StructureId, s => s.VehRegNo, typeof(string));

            private static IStructureSchema _structureSchema;
            private static VehicleWithGuidId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_guid_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithGuidId>();
                _structures = new[]
                {
                    new VehicleWithGuidId { VehRegNo = "ABC123" },
                    new VehicleWithGuidId { VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_items_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithGuidId>(2);

            It should_have_inserted_structures_correctly = () => TestContext.Database.should_have_identical_structures<VehicleWithGuidId>(_structures.ToArray());

            It should_have_inserted_values_in_unique_table = () =>
            {
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithGuidId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo);
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithGuidId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo);
            };

            It should_have_inserted_values_in_indexes_table = () =>
            {
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithGuidId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo, typeof(string));
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithGuidId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo, typeof(string));
            };

            private static IStructureSchema _structureSchema;
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
#if SqlCe4Provider
                CaughtException.Message.ShouldStartWith("A duplicate value cannot be inserted into a unique index. [ Table name = VehicleWithGuidIdUniques,Constraint name = UQ_VehicleWithGuidIdUniques ]");
#else
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithGuidIdUniques' with unique index 'UQ_VehicleWithGuidIdUniques'.");
#endif
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithGuidId>(1);

            It should_still_have_inserted_the_first_item = () => TestContext.Database.should_have_identical_structures<VehicleWithGuidId>(new[] { _orgStructure });

            private static VehicleWithGuidId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_string_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithStringId>();
                _structure = new VehicleWithStringId { StructureId = "A123", VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithStringId>(1);

            It should_have_inserted_structure_correctly = () => TestContext.Database.should_have_identical_structures(_structure);

            It should_have_inserted_value_in_unique_table = () => TestContext.DbHelper
                .UniquesTableHasMember<VehicleWithStringId>(_structureSchema, _structure.StructureId, s => s.VehRegNo);

            It should_have_inserted_value_in_indexes_table = () => TestContext.DbHelper
                .should_have_stored_member_in_indexes_table<VehicleWithStringId>(_structureSchema, _structure.StructureId, s => s.VehRegNo, typeof(string));

            private static IStructureSchema _structureSchema;
            private static VehicleWithStringId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_string_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithStringId>();
                _structures = new[]
                {
                    new VehicleWithStringId { StructureId = "A1", VehRegNo = "ABC123" },
                    new VehicleWithStringId { StructureId = "A2", VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_items_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithStringId>(2);

            It should_have_inserted_structures_correctly = () => TestContext.Database.should_have_identical_structures<VehicleWithStringId>(_structures.ToArray());

            It should_have_inserted_values_in_unique_table = () =>
            {
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithStringId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo);
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithStringId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo);
            };

            It should_have_inserted_values_in_indexes_table = () =>
            {
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithStringId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo, typeof(string));
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithStringId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo, typeof(string));
            };

            private static IStructureSchema _structureSchema;
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
#if SqlCe4Provider
                CaughtException.Message.ShouldStartWith("A duplicate value cannot be inserted into a unique index. [ Table name = VehicleWithStringIdUniques,Constraint name = UQ_VehicleWithStringIdUniques ]");
#else
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithStringIdUniques' with unique index 'UQ_VehicleWithStringIdUniques'.");
#endif
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithStringId>(1);

            It should_still_have_inserted_the_first_item = () => TestContext.Database.should_have_identical_structures<VehicleWithStringId>(new[] { _orgStructure });

            private static VehicleWithStringId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_identity_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithIdentityId>();
                _structure = new VehicleWithIdentityId { VehRegNo = "ABC123" };
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithIdentityId>(1);

            It should_have_inserted_structure_correctly = () => TestContext.Database.should_have_identical_structures(_structure);

            It should_have_inserted_value_in_unique_table = () => TestContext.DbHelper
                .UniquesTableHasMember<VehicleWithIdentityId>(_structureSchema, _structure.StructureId, s => s.VehRegNo);

            It should_have_inserted_value_in_indexes_table = () => TestContext.DbHelper
                .should_have_stored_member_in_indexes_table<VehicleWithIdentityId>(_structureSchema, _structure.StructureId, s => s.VehRegNo, typeof(string));

            private static IStructureSchema _structureSchema;
            private static VehicleWithIdentityId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_identity_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithIdentityId>();
                _structures = new[]
                {
                    new VehicleWithIdentityId { VehRegNo = "ABC123" },
                    new VehicleWithIdentityId { VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_items_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithIdentityId>(2);

            It should_have_inserted_structures_correctly = () => TestContext.Database.should_have_identical_structures<VehicleWithIdentityId>(_structures.ToArray());

            It should_have_inserted_values_in_unique_table = () =>
            {
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithIdentityId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo);
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithIdentityId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo);
            };

            It should_have_inserted_values_in_indexes_table = () =>
            {
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithIdentityId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo, typeof(string));
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithIdentityId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo, typeof(string));
            };

            private static IStructureSchema _structureSchema;
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
#if SqlCe4Provider
                CaughtException.Message.ShouldStartWith("A duplicate value cannot be inserted into a unique index. [ Table name = VehicleWithIdentityIdUniques,Constraint name = UQ_VehicleWithIdentityIdUniques ]");
#else
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithIdentityIdUniques' with unique index 'UQ_VehicleWithIdentityIdUniques'.");
#endif
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithIdentityId>(1);

            It should_still_have_inserted_the_first_item = () => TestContext.Database.should_have_identical_structures<VehicleWithIdentityId>(new[] { _orgStructure });

            private static VehicleWithIdentityId _orgStructure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_one_unique_per_type_big_identity_entity : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithBigIdentityId>();
                _structure = new VehicleWithBigIdentityId {VehRegNo = "ABC123"};
                TestContext.Database.UseOnceTo().Insert(_structure);
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithBigIdentityId>(1);

            It should_have_inserted_structure_correctly = () => TestContext.Database.should_have_identical_structures(_structure);

            It should_have_inserted_value_in_unique_table = () => TestContext.DbHelper
                .UniquesTableHasMember<VehicleWithBigIdentityId>(_structureSchema, _structure.StructureId, s => s.VehRegNo);

            It should_have_inserted_value_in_indexes_table = () => TestContext.DbHelper
                .should_have_stored_member_in_indexes_table<VehicleWithBigIdentityId>(_structureSchema, _structure.StructureId, s => s.VehRegNo, typeof(string));

            private static IStructureSchema _structureSchema;
            private static VehicleWithBigIdentityId _structure;
        }

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_inserting_two_unique_per_type_big_identity_entities : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<VehicleWithBigIdentityId>();
                _structures = new[]
                {
                    new VehicleWithBigIdentityId { VehRegNo = "ABC123" },
                    new VehicleWithBigIdentityId { VehRegNo = "ABC321" }
                };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            It should_have_two_items_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithBigIdentityId>(2);

            It should_have_inserted_structures_correctly = () => TestContext.Database.should_have_identical_structures<VehicleWithBigIdentityId>(_structures.ToArray());

            It should_have_inserted_values_in_unique_table = () =>
            {
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithBigIdentityId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo);
                TestContext.DbHelper.UniquesTableHasMember<VehicleWithBigIdentityId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo);
            };

            It should_have_inserted_values_in_indexes_table = () =>
            {
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithBigIdentityId>(_structureSchema, _structures[0].StructureId, s => s.VehRegNo, typeof(string));
                TestContext.DbHelper.should_have_stored_member_in_indexes_table<VehicleWithBigIdentityId>(_structureSchema, _structures[1].StructureId, s => s.VehRegNo, typeof(string));
            };

            private static IStructureSchema _structureSchema;
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
#if SqlCe4Provider
                CaughtException.Message.ShouldStartWith("A duplicate value cannot be inserted into a unique index. [ Table name = VehicleWithBigIdentityIdUniques,Constraint name = UQ_VehicleWithBigIdentityIdUniques ]");
#else
                CaughtException.Message.ShouldStartWith("Cannot insert duplicate key row in object 'dbo.VehicleWithBigIdentityIdUniques' with unique index 'UQ_VehicleWithBigIdentityIdUniques'.");
#endif
            };

            It should_have_one_item_inserted = () => TestContext.Database.should_have_X_num_of_items<VehicleWithBigIdentityId>(1);

            It should_still_have_inserted_the_first_item = () => TestContext.Database.should_have_identical_structures<VehicleWithBigIdentityId>(new[] { _orgStructure });

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

        [Subject(typeof(ISession), "Insert (unique per type)")]
        public class when_empty_collection_with_items_having_member_with_unique_constraint : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<WithCollectionOfUnqies>();
                _orgStructure = new WithCollectionOfUnqies { Items = new List<WithUniqueStringPerType>() };
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