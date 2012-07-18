using System;
using Machine.Specifications;
using SisoDb.DbSchema;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    class DropStructureSets
    {
        [Subject(typeof(ISisoDatabase), "Drop structure sets")]
        public class when_two_different_sets_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyClass1>();
                _structureSchema1 = TestContext.Database.StructureSchemas.GetSchema<MyClass1>();
                _structureSchema2 = TestContext.Database.StructureSchemas.GetSchema<MyClass1>();
            };

            Because of =
                () => TestContext.Database.DropStructureSets(new[] { typeof(MyClass1), typeof(MyClass2) });

            It should_have_dropped_first_sets_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema1.GetStructureTableName()).ShouldBeFalse();

			It should_have_dropped_first_sets_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

			It should_have_dropped_first_sets_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

			It should_have_dropped_first_sets_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

			It should_have_dropped_first_sets_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

			It should_have_dropped_first_sets_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

			It should_have_dropped_first_sets_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_have_dropped_first_sets_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema1.GetUniquesTableName()).ShouldBeFalse();

            It should_have_dropped_second_sets_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema2.GetStructureTableName()).ShouldBeFalse();

			It should_have_dropped_second_sets_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

			It should_have_dropped_second_sets_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

			It should_have_dropped_second_sets_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

			It should_have_dropped_second_sets_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

			It should_have_dropped_second_sets_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

			It should_have_dropped_second_sets_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_have_dropped_second_sets_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema2.GetUniquesTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema1;
            private static IStructureSchema _structureSchema2;

            private class MyClass1
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }

            private class MyClass2
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }

        [Subject(typeof(ISisoDatabase), "Drop structure sets")]
        public class when_two_different_sets_exists_in_a_recreated_database_instance : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyClass1>();
                _structureSchema1 = TestContext.Database.StructureSchemas.GetSchema<MyClass1>();
                _structureSchema2 = TestContext.Database.StructureSchemas.GetSchema<MyClass1>();

                //Recreate
                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.DropStructureSets(new[] { typeof(MyClass1), typeof(MyClass2) });

            It should_have_dropped_first_sets_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema1.GetStructureTableName()).ShouldBeFalse();

			It should_have_dropped_first_sets_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

			It should_have_dropped_first_sets_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

			It should_have_dropped_first_sets_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

			It should_have_dropped_first_sets_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

			It should_have_dropped_first_sets_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

			It should_have_dropped_first_sets_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema1.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_have_dropped_first_sets_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema1.GetUniquesTableName()).ShouldBeFalse();

            It should_have_dropped_second_sets_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema2.GetStructureTableName()).ShouldBeFalse();

			It should_have_dropped_second_sets_integers_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Integers)).ShouldBeFalse();

			It should_have_dropped_second_sets_fractals_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Fractals)).ShouldBeFalse();

			It should_have_dropped_second_sets_booleans_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Booleans)).ShouldBeFalse();

			It should_have_dropped_second_sets_dates_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Dates)).ShouldBeFalse();

			It should_have_dropped_second_sets_guids_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Guids)).ShouldBeFalse();

			It should_have_dropped_second_sets_strings_indexes_table =
				() => TestContext.DbHelper.TableExists(_structureSchema2.GetIndexesTableNameFor(IndexesTypes.Strings)).ShouldBeFalse();

            It should_have_dropped_second_sets_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema2.GetUniquesTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema1;
            private static IStructureSchema _structureSchema2;

            private class MyClass1
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }

            private class MyClass2
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }
    }
}