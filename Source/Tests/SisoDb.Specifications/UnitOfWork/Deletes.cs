using System;
using System.Collections.Generic;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.Testing.Steps;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.UnitOfWork
{
    namespace Deletes
    {
        [Subject(typeof(IUnitOfWork), "Delete by query")]
        public class when_guiditem_and_deleting_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using(var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByQuery<GuidItem>(i => i.Value >= _structures[1].Value && i.Value <= _structures[2].Value);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by query")]
        public class when_identityitem_and_deleting_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByQuery<IdentityItem>(i => i.Value >= _structures[1].Value && i.Value <= _structures[2].Value);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id")]
        public class when_guiditem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<GuidItem>(_structures[1].StructureId);
                    uow.DeleteById<GuidItem>(_structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id (uniques)")]
        public class when_uniqueguiditem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueGuidItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<UniqueGuidItem>(_structures[1].StructureId);
                    uow.DeleteById<UniqueGuidItem>(_structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueGuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueGuidItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id")]
        public class when_identityitem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<IdentityItem>(_structures[1].StructureId);
                    uow.DeleteById<IdentityItem>(_structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left = 
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id (uniques)")]
        public class when_uniqueidentityitem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueIdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<UniqueIdentityItem>(_structures[1].StructureId);
                    uow.DeleteById<UniqueIdentityItem>(_structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueIdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id")]
        public class when_bigidentityitem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<BigIdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<BigIdentityItem>(_structures[1].StructureId);
                    uow.DeleteById<BigIdentityItem>(_structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<BigIdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id (uniques)")]
        public class when_uniquebigidentityitem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueBigIdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteById<UniqueBigIdentityItem>(_structures[1].StructureId);
                    uow.DeleteById<UniqueBigIdentityItem>(_structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueBigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueBigIdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id")]
        public class when_guiditem_and_deleting_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context = 
                () => TestContext = TestContextFactory.Create();
            
            Because of = () =>
            {
                CaughtException = Catch.Exception(() => 
                {
                    using (var uow = TestContext.Database.CreateUnitOfWork())
                    {
                        uow.DeleteById<GuidItem>(Guid.Parse("F875F861-24DC-420C-988A-196977A21C43"));
                        uow.Commit();
                    }
                });
            };

            It should_not_have_failed =
                () => CaughtException.ShouldBeNull();
        }

        [Subject(typeof(IUnitOfWork), "Delete by id")]
        public class when_identityitem_and_deleting_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context =
                () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var uow = TestContext.Database.CreateUnitOfWork())
                    {
                        uow.DeleteById<IdentityItem>(1);
                        uow.Commit();
                    }
                });
            };

            It should_not_have_failed =
                () => CaughtException.ShouldBeNull();
        }

        [Subject(typeof(IUnitOfWork), "Delete by ids")]
        public class when_guiditem_and_deleting_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByIds<GuidItem>(_structures[1].StructureId, _structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by ids")]
        public class when_identityitem_and_deleting_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByIds<IdentityItem>(_structures[1].StructureId, _structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by ids")]
        public class when_bigidentityitem_and_deleting_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<BigIdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByIds<BigIdentityItem>(_structures[1].StructureId, _structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<BigIdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id interval")]
        public class when_guiditem_and_deleting_two_of_four_items_using_id_interval : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByIdInterval<GuidItem>(_structures[1].StructureId, _structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id interval")]
        public class when_identityitem_and_deleting_two_of_four_items_using_id_interval : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByIdInterval<IdentityItem>(_structures[1].StructureId, _structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(IUnitOfWork), "Delete by id interval")]
        public class when_bigidentityitem_and_deleting_two_of_four_items_using_id_interval : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<BigIdentityItem>();
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.DeleteByIdInterval<BigIdentityItem>(_structures[1].StructureId, _structures[2].StructureId);
                    uow.Commit();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<BigIdentityItem> _structures;
        }
    }
}