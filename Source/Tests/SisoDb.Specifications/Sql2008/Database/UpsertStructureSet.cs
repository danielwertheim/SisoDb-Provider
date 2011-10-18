using System;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Sql2008;
using SisoDb.Structures;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.Database
{
    namespace UpsertStructureSet
    {
        [Subject(typeof(Sql2008Database), "Upsert structure set")]
        public class when_using_generics_and_no_set_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.DropStructureSet<MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet<MyClass>();

            It should_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

            It should_have_created_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableName()).ShouldBeTrue();

            It should_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(Sql2008Database), "Upsert structureset")]
        public class when_using_type_and_no_set_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.DropStructureSet<MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet(typeof(MyClass));

            It should_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

            It should_have_created_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableName()).ShouldBeTrue();

            It should_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        public class MyClass
        {
            public Guid StructureId { get; set; }

            public string IndexableMember1 { get; set; }

            public int IndexableMember2 { get; set; }
        }
    }
}