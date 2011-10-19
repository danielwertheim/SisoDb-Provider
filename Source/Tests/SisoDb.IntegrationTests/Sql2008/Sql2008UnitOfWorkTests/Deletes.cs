using System;
using System.Collections.Generic;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Sql2008;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.IntegrationTests.Sql2008.Sql2008UnitOfWorkTests
{
    namespace Deletes
    {
        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_guiditem_and_deleting_first_item_of_two_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertUniqueGuidItems(2);
                _structureIdToDelete = _structures[0].StructureId;
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueGuidItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<UniqueGuidItem>(_structureIdToDelete);
                    uow.Commit();
                }
            };

            It should_have_been_deleted_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structureIdToDelete);

            It should_have_been_deleted_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structureIdToDelete);

            It should_have_been_deleted_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structureIdToDelete);

            private static IList<UniqueGuidItem> _structures;
            private static IStructureSchema _structureSchema;
            private static Guid _structureIdToDelete;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_identityitem_and_deleting_first_item_of_two_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertUniqueIdentityItems(2);
                _structureIdToDelete = _structures[0].StructureId;
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueIdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<UniqueIdentityItem>(_structureIdToDelete);
                    uow.Commit();
                }
            };

            It should_have_been_deleted_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structureIdToDelete);

            It should_have_been_deleted_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structureIdToDelete);

            It should_have_been_deleted_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structureIdToDelete);

            private static IList<UniqueIdentityItem> _structures;
            private static IStructureSchema _structureSchema;
            private static int _structureIdToDelete;
        }
    }
}