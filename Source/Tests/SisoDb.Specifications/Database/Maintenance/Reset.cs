using System;
using Machine.Specifications;
using SisoDb.DbSchema;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database.Maintenance
{
    class Reset
    {
        [Subject(typeof (ISisoDatabaseMaintenance), "Reset")]
        public class when_two_structuresets_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<StructureSetOne>();
                TestContext.Database.UpsertStructureSet<StructureSetTwo>();
                _structureSchemaForSetOne = TestContext.Database.StructureSchemas.GetSchema<StructureSetOne>();
                _structureSchemaForSetTwo = TestContext.Database.StructureSchemas.GetSchema<StructureSetTwo>();
            };

            Because of =
                () => TestContext.Database.Maintenance.Reset();

            It should_have_cleared_identities_table =
                () => TestContext.DbHelper.RowCount("SisoDbIdentities").ShouldEqual(0);

            It should_have_dropped_structure_table_forSetOne =
                () => TestContext.DbHelper.TableExists(_structureSchemaForSetOne.GetStructureTableName()).ShouldBeFalse();

            It should_have_dropped_structure_table_forSetTwo =
                () => TestContext.DbHelper.TableExists(_structureSchemaForSetTwo.GetStructureTableName()).ShouldBeFalse();

            It should_have_dropped_uniques_table_forSetOne =
                () => TestContext.DbHelper.TableExists(_structureSchemaForSetOne.GetUniquesTableName()).ShouldBeFalse();

            It should_have_dropped_uniques_table_forSetTwo =
                () => TestContext.DbHelper.TableExists(_structureSchemaForSetTwo.GetUniquesTableName()).ShouldBeFalse();

            It should_have_dropped_all_indexes_tables_for_SetOne = 
                () => TestContext.DbHelper.TablesExists(_structureSchemaForSetOne.GetIndexesTableNames().All).ShouldBeFalse();

            It should_have_dropped_all_indexes_tables_for_SetTwo =
                () => TestContext.DbHelper.TablesExists(_structureSchemaForSetTwo.GetIndexesTableNames().All).ShouldBeFalse();

            private static IStructureSchema _structureSchemaForSetOne;
            private static IStructureSchema _structureSchemaForSetTwo;
        }

        private class StructureSetOne
        {
            public Guid StructureId { get; set; }
            public string Value { get; set; }
        }

        private class StructureSetTwo
        {
            public int StructureId { get; set; }
            public string Value { get; set; }
        }
    }
}