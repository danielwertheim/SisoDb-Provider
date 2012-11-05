using System;
using Machine.Specifications;
using SisoDb.DbSchema;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    class DropStructureSet
    {
        [Subject(typeof(ISisoDatabase), "Drop structure set")]
        public class when_set_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyClass>();
            };

            Because of = 
                () => TestContext.Database.DropStructureSet<MyClass>();

            It should_have_dropped_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeFalse();

            It should_have_dropped_integers_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

			It should_have_dropped_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

			It should_have_dropped_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

			It should_have_dropped_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();
			
			It should_have_dropped_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

			It should_have_dropped_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_have_dropped_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema;

            private class MyClass
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }

        [Subject(typeof(ISisoDatabase), "Drop structure set")]
        public class when_set_exists_in_a_recreated_database_instance : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyClass>();

                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.DropStructureSet<MyClass>();

            It should_have_dropped_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeFalse();

			It should_have_dropped_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

			It should_have_dropped_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

			It should_have_dropped_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

			It should_have_dropped_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

			It should_have_dropped_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

			It should_have_dropped_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_have_dropped_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema;

            private class MyClass
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }
    }
}