using System;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.DbSchema;
using System.Linq;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.Database.Maintenance
{
    class SetRowIdsOn
    {
        [Subject(typeof(ISisoDatabaseMaintenance), "SetRowIdsOn")]
        public class when_tables_for_two_different_structures_with_no_row_ids_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyFoo>();
                TestContext.Database.UpsertStructureSet<MyBar>();

                _myFooSchema = TestContext.Database.StructureSchemas.GetSchema<MyFoo>();
                _myBarSchema = TestContext.Database.StructureSchemas.GetSchema<MyBar>();
            };

            Because of = ()
                => TestContext.Database.Maintenance.SetRowIdsOn();

            It should_have_added_row_id_to_foo_structure_table =
                () => TestContext.DbHelper.should_have_column(_myFooSchema.GetStructureTableName(), StructureStorageSchema.Fields.RowId.Name);

            It should_have_added_row_id_to_bar_structure_table =
                () => TestContext.DbHelper.should_have_column(_myBarSchema.GetStructureTableName(), StructureStorageSchema.Fields.RowId.Name);

            It should_have_added_row_id_to_foo_uniques_table =
                () => TestContext.DbHelper.should_have_column(_myFooSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.RowId.Name);

            It should_have_added_row_id_to_bar_uniques_table =
                () => TestContext.DbHelper.should_have_column(_myBarSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.RowId.Name);

            It should_have_added_row_id_to_foo_indexes_tables =
                () => TestContext.DbHelper.should_have_column_in_all_indexestables(_myFooSchema.GetIndexesTableNames(), IndexStorageSchema.Fields.RowId.Name);

            It should_have_added_row_id_to_bar_indexes_tables =
                () => TestContext.DbHelper.should_have_column_in_all_indexestables(_myBarSchema.GetIndexesTableNames(), IndexStorageSchema.Fields.RowId.Name);

            private static IStructureSchema _myFooSchema, _myBarSchema;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "SetRowIdsOn")]
        public class when_setrowidson_has_been_called_insert_can_be_performed : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyFoo>();
                TestContext.Database.UpsertStructureSet<MyBar>();

                TestContext.Database.Maintenance.SetRowIdsOn();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _myFoo = new MyFoo { StringValue1 = "This is Foo", IntValue1 = 42 };
                    _myBar = new MyBar { StringValue1 = "This is Bar", IntValue1 = 42 };
                    session
                        .Insert(_myFoo)
                        .Insert(_myBar);
                }
            };

            It should_have_inserted_item_to_foo =
                () => TestContext.Database.should_have_identical_structures(_myFoo);

            It should_have_inserted_item_to_bar =
                () => TestContext.Database.should_have_identical_structures(_myBar);

            private static MyFoo _myFoo;
            private static MyBar _myBar;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "SetRowIdsOn")]
        public class when_setrowidson_has_been_called_insertmany_can_be_performed : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<MyFoo>();
                TestContext.Database.UpsertStructureSet<MyBar>();

                TestContext.Database.Maintenance.SetRowIdsOn();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _myFoos = new[]
                    {
                        new MyFoo { StringValue1 = "This is Foo string 1", StringValue2 = "This is Foo string 2", IntValue1 = 42, IntValue2 = -42},
                        new MyFoo { StringValue1 = "This is Foo string 1", StringValue2 = "This is Foo string 2", IntValue1 = 42, IntValue2 = -42}
                    };
                    _myBars = new[]
                    {
                        new MyBar { StringValue1 = "This is Bar string 1", StringValue2 = "This is Bar string 2", IntValue1 = 42, IntValue2 = -42},
                        new MyBar { StringValue1 = "This is Bar string 1", StringValue2 = "This is Bar string 2", IntValue1 = 42, IntValue2 = -42}
                    };
                    session
                        .InsertMany(_myFoos)
                        .InsertMany(_myBars);
                }
            };

            It should_have_inserted_item_to_foo =
                () => TestContext.Database.should_have_identical_structures(_myFoos);

            It should_have_inserted_item_to_bar =
                () => TestContext.Database.should_have_identical_structures(_myBars);

            private static MyFoo[] _myFoos;
            private static MyBar[] _myBars;
        }

        private class MyFoo
        {
            public Guid Id { get; set; }
            public string StringValue1 { get; set; }
            public string StringValue2 { get; set; }
            public int IntValue1 { get; set; }
            public int IntValue2 { get; set; }
        }

        private class MyBar
        {
            public int Id { get; set; }
            public string StringValue1 { get; set; }
            public string StringValue2 { get; set; }
            public int IntValue1 { get; set; }
            public int IntValue2 { get; set; }
        }
    }
}