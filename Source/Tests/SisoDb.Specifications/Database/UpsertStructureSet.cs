using System;
using System.Linq;
using Machine.Specifications;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    class UpsertStructureSet
    {
        [Subject(typeof(ISisoDatabase), "Upsert structure set")]
        public class when_using_generics_and_no_set_exists_and_allow_upsert_of_schemas_is_false : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.Settings.AllowUpsertsOfSchemas = false;
                TestContext.Database.DropStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();

            It should_not_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeFalse();

            It should_not_have_created_integers_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

            It should_not_have_created_fractals_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

            It should_not_have_created_booleans_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

            It should_not_have_created_dates_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

            It should_not_have_created_guids_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

            It should_not_have_created_strings_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_not_have_created_texts_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Texts)).ShouldBeFalse();

            It should_not_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoDatabase), "Upsert structureset")]
        public class when_using_type_and_no_set_exists_and_allow_upsert_of_schemas_is_false : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.Settings.AllowUpsertsOfSchemas = false;
                TestContext.Database.DropStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet(typeof(OrgModel.MyClass));

            It should_not_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeFalse();

            It should_not_have_created_integers_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

            It should_not_have_created_fractals_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

            It should_not_have_created_booleans_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

            It should_not_have_created_dates_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

            It should_not_have_created_guids_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

            It should_not_have_created_strings_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_not_have_created_texts_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Texts)).ShouldBeFalse();

            It should_not_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoDatabase), "Upsert structure set")]
        public class when_using_generics_and_no_set_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.DropStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();

            It should_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

            It should_have_created_integers_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeTrue();

			It should_have_created_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeTrue();

			It should_have_created_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeTrue();

			It should_have_created_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeTrue();

			It should_have_created_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeTrue();

			It should_have_created_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeTrue();

            It should_have_created_texts_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Texts)).ShouldBeTrue();

            It should_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoDatabase), "Upsert structureset")]
        public class when_using_type_and_no_set_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.DropStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet(typeof(OrgModel.MyClass));

            It should_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

			It should_have_created_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeTrue();

			It should_have_created_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeTrue();

			It should_have_created_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeTrue();

			It should_have_created_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeTrue();

			It should_have_created_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeTrue();

			It should_have_created_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeTrue();

            It should_have_created_texts_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Texts)).ShouldBeTrue();

            It should_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof (ISisoDatabase), "Upsert structureset")]
        public class when_model_has_dropped_column : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();

                using(var session = TestContext.Database.BeginSession())
                {
                    session.Insert(new OrgModel.MyClass
                    {
                        IndexableMember1 = "My string", IndexableMember2 = 42
                    });
                }

                TestContext = TestContextFactory.Create();
            };

            Because of = 
                () => TestContext.Database.UpsertStructureSet<DroppedColumnModel.MyClass>();

            It should_have_dropped_all_indexes_for_dropped_member = 
                () => TestContext.DbHelper.RowCount(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings),
                    "[{0}]='IndexableMember1'".Inject(IndexStorageSchema.Fields.MemberPath.Name)).ShouldEqual(0);

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoDatabase), "Upsert structureset")]
        public class when_structure_has_new_structure_id_type : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();

                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet<NewIdTypeModel.MyClass>();

            It should_still_have_structures_table = () =>
                TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

            It should_still_have_original_structureid_in_structures_table = () =>
            {
                var columns = TestContext.DbHelper.GetColumns(_structureSchema.GetUniquesTableName());
                var column = columns.SingleOrDefault(c => c.Name == StructureStorageSchema.Fields.Id.Name);

                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };
			
			It should_still_have_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeTrue();

			It should_still_have_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeTrue();

			It should_still_have_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeTrue();

			It should_still_have_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeTrue();

			It should_still_have_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeTrue();

			It should_still_have_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeTrue();

            It should_still_have_texts_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Texts)).ShouldBeTrue();

            It should_still_have_original_structureid_in_indexes_table = () =>
            {
                var columns = TestContext.DbHelper.GetColumns(_structureSchema.GetUniquesTableName());
                var column = columns.SingleOrDefault(c => c.Name == IndexStorageSchema.Fields.StructureId.Name);
                
                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            It should_still_have_uniques_table = () =>
                TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            It should_still_have_original_structureid_in_uniques_table = () =>
            {
                var columns = TestContext.DbHelper.GetColumns(_structureSchema.GetUniquesTableName());
                var column = columns.SingleOrDefault(c => c.Name == UniqueStorageSchema.Fields.StructureId.Name);

                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            It should_still_have_original_uqstructureid_in_uniques_table = () =>
            {
                var columns = TestContext.DbHelper.GetColumns(_structureSchema.GetUniquesTableName());
                var column = columns.SingleOrDefault(c => c.Name == UniqueStorageSchema.Fields.UqStructureId.Name);
                
                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            private static IStructureSchema _structureSchema;
        }

        class OrgModel
        {
            public class MyClass
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }

        class NewIdTypeModel
        {
            public class MyClass
            {
                public int StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }

		class DroppedColumnModel
        {
            public class MyClass
            {
                public int StructureId { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }
    }
}