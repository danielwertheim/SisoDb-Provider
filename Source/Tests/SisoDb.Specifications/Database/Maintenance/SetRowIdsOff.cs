using System;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.DbSchema;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.Database.Maintenance
{
    class SetRowIdsOff
    {
        [Subject(typeof(ISisoDatabaseMaintenance), "SetRowIdsOff")]
        public class when_tables_for_two_different_structures_with_row_ids_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyFoo>();
                TestContext.Database.UpsertStructureSet<MyBar>();

                _myFooSchema = TestContext.Database.StructureSchemas.GetSchema<MyFoo>();
                _myBarSchema = TestContext.Database.StructureSchemas.GetSchema<MyBar>();

                TestContext.Database.Maintenance.SetRowIdsOn();
            };

            Because of = () 
                => TestContext.Database.Maintenance.SetRowIdsOff();

            It should_not_have_added_row_id_to_foo_structure_table =
                () => TestContext.DbHelper.should_not_have_column(_myFooSchema.GetStructureTableName(), StructureStorageSchema.Fields.RowId.Name);

            It should_not_have_added_row_id_to_bar_structure_table =
                () => TestContext.DbHelper.should_not_have_column(_myBarSchema.GetStructureTableName(), StructureStorageSchema.Fields.RowId.Name);

            It should_not_have_added_row_id_to_foo_uniques_table =
                () => TestContext.DbHelper.should_not_have_column(_myFooSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.RowId.Name);

            It should_have_added_row_id_to_bar_uniques_table = 
                () => TestContext.DbHelper.should_not_have_column(_myBarSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.RowId.Name);

            It should_not_have_added_row_id_to_foo_indexes_tables = 
                () => TestContext.DbHelper.should_not_have_column_in_any_indexestables(_myFooSchema.GetIndexesTableNames(), IndexStorageSchema.Fields.RowId.Name);

            It should_not_have_added_row_id_to_bar_indexes_tables =
                () => TestContext.DbHelper.should_not_have_column_in_any_indexestables(_myBarSchema.GetIndexesTableNames(), IndexStorageSchema.Fields.RowId.Name);

            private static IStructureSchema _myFooSchema, _myBarSchema;
        }

        private class MyFoo
        {
            public Guid Id { get; set; }
        }

        private class MyBar
        {
            public int Id { get; set; }
        }
    }
}