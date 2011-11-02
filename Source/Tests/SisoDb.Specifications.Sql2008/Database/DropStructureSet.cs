using System;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Sql2008;
using SisoDb.Structures;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.Database
{
    namespace DropStructureSet
    {
        [Subject(typeof(Sql2008Database), "Drop structure set")]
        public class when_set_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.UpsertStructureSet<MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyClass>();
            };

            Because of = 
                () => TestContext.Database.DropStructureSet<MyClass>();

            It should_have_dropped_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeFalse();

            It should_have_dropped_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableName()).ShouldBeFalse();

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