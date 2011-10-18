using System;
using System.Linq;
using Machine.Specifications;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
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
                TestContext.Database.DropStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();

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
                TestContext.Database.DropStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet(typeof(OrgModel.MyClass));

            It should_have_created_structure_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

            It should_have_created_indexes_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableName()).ShouldBeTrue();

            It should_have_created_uniques_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof (Sql2008Database), "Upsert structureset")]
        public class when_model_has_dropped_column : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();

                using(var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.Insert(new OrgModel.MyClass
                    {
                        IndexableMember1 = "My string", IndexableMember2 = 42
                    });
                    uow.Commit();
                }

                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
            };

            Because of = 
                () => TestContext.Database.UpsertStructureSet<DroppedColumnModel.MyClass>();

            It should_have_dropped_all_indexes_for_dropped_member = 
                () => TestContext.DbHelper.RowCount(
                    _structureSchema.GetIndexesTableName(),
                    "[{0}]='IndexableMember1'".Inject(IndexStorageSchema.Fields.MemberPath.Name)).ShouldEqual(0);

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(Sql2008Database), "Upsert structureset")]
        public class when_structure_has_new_structure_id_type : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                TestContext.Database.UpsertStructureSet<OrgModel.MyClass>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<OrgModel.MyClass>();

                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
            };

            Because of =
                () => TestContext.Database.UpsertStructureSet<NewIdTypeModel.MyClass>();

            It should_still_have_structures_table = () =>
                TestContext.DbHelper.TableExists(_structureSchema.GetStructureTableName()).ShouldBeTrue();

            It should_still_have_original_structureid_in_structures_table = () =>
            {
                DbColumn column;
                using (var dbClient = TestContext.CreateNonTransactionalDbClient())
                {
                    var columns = dbClient.GetColumns(_structureSchema.GetStructureTableName());
                    column = columns.SingleOrDefault(c => c.Name == StructureStorageSchema.Fields.Id.Name);
                }
                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            It should_still_have_indexes_table = () =>
                TestContext.DbHelper.TableExists(_structureSchema.GetIndexesTableName()).ShouldBeTrue();

            It should_still_have_original_structureid_in_indexes_table = () =>
            {
                DbColumn column;
                using (var dbClient = TestContext.CreateNonTransactionalDbClient())
                {
                    var columns = dbClient.GetColumns(_structureSchema.GetIndexesTableName());
                    column = columns.SingleOrDefault(c => c.Name == IndexStorageSchema.Fields.StructureId.Name);
                }
                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            It should_still_have_uniques_table = () =>
                TestContext.DbHelper.TableExists(_structureSchema.GetUniquesTableName()).ShouldBeTrue();

            It should_still_have_original_structureid_in_uniques_table = () =>
            {
                DbColumn column;
                using (var dbClient = TestContext.CreateNonTransactionalDbClient())
                {
                    var columns = dbClient.GetColumns(_structureSchema.GetUniquesTableName());
                    column = columns.SingleOrDefault(c => c.Name == UniqueStorageSchema.Fields.StructureId.Name);
                }
                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            It should_still_have_original_uqstructureid_in_uniques_table = () =>
            {
                DbColumn column;
                using (var dbClient = TestContext.CreateNonTransactionalDbClient())
                {
                    var columns = dbClient.GetColumns(_structureSchema.GetUniquesTableName());
                    column = columns.SingleOrDefault(c => c.Name == UniqueStorageSchema.Fields.UqStructureId.Name);
                }
                column.ShouldNotBeNull();
                column.DbDataType.ShouldEqual("uniqueidentifier");
            };

            private static IStructureSchema _structureSchema;

            private static Exception _exception;
        }

        namespace OrgModel
        {
            public class MyClass
            {
                public Guid StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }

        namespace NewIdTypeModel
        {
            public class MyClass
            {
                public int StructureId { get; set; }

                public string IndexableMember1 { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }

        namespace DroppedColumnModel
        {
            public class MyClass
            {
                public int StructureId { get; set; }

                public int IndexableMember2 { get; set; }
            }
        }
    }
}