using System;
using Machine.Specifications;
using SisoDb.DbSchema;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database.Maintenance
{
    class RenameStructure
    {
        [Subject(typeof(ISisoDatabaseMaintenance), "RenameStructure")]
        public class when_structuresets_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<StructureSet>();
                _orgStructureSchema = TestContext.Database.StructureSchemas.GetSchema<StructureSet>();
                _newStructureName = "New" + _orgStructureSchema.Name;
            };

            Because of =
                () => TestContext.Database.Maintenance.RenameStructure(_orgStructureSchema.Name, _newStructureName);

            It should_have_no_old_structure_table =
                () => TestContext.DbHelper.TableExists(_orgStructureSchema.GetStructureTableName()).ShouldBeFalse();

            It should_have_no_old_uniques_table =
                () => TestContext.DbHelper.TableExists(_orgStructureSchema.GetUniquesTableName()).ShouldBeFalse();

            It should_have_no_old_indexes_tables =
                () => TestContext.DbHelper.TablesExists(_orgStructureSchema.GetIndexesTableNames().All).ShouldBeFalse();

            It should_have_new_structure_table =
                () => TestContext.DbHelper.TableExists(DbSchemaInfo.GenerateStructureTableName(_newStructureName)).ShouldBeTrue();

            It should_have_new_uniques_table =
                () => TestContext.DbHelper.TableExists(DbSchemaInfo.GenerateUniquesTableName(_newStructureName)).ShouldBeTrue();

            It should_have_new_indexes_tables =
                () => TestContext.DbHelper.TablesExists(new IndexesTableNames(_newStructureName).All).ShouldBeTrue();

            private static IStructureSchema _orgStructureSchema;
            private static string _newStructureName;
        }

        private class StructureSet
        {
            public Guid StructureId { get; set; }
            public string Value { get; set; }
        }
    }
}