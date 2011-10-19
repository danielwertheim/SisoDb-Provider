using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Sql2008.UnitOfWork
{
    namespace Deletes
    {
        [Subject(typeof(Sql2008UnitOfWork), "Delete by query")]
        public class when_guiditem_and_deleting_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertGuidItems(4);
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
                () => TestContext.Database.should_only_have_X_items_left<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Delete by query")]
        public class when_identityitem_and_deleting_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertIdentityItems(4);
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
                () => TestContext.Database.should_only_have_X_items_left<IdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_guiditem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertGuidItems(4);
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
                () => TestContext.Database.should_only_have_X_items_left<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_identityitem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertIdentityItems(4);
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
                () => TestContext.Database.should_only_have_X_items_left<IdentityItem>(2);

            It should_have_first_and_last_item_left = 
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_bigidentityitem_and_deleting_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structures = TestContext.Database.InsertBigIdentityItems(4);
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
                () => TestContext.Database.should_only_have_X_items_left<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            private static IList<BigIdentityItem> _structures;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_guiditem_and_deleting_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context = 
                () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
            
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

        [Subject(typeof(Sql2008UnitOfWork), "Delete by id")]
        public class when_identityitem_and_deleting_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context =
                () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

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
    }
}