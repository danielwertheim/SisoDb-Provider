using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

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
                () => TestContext.Database.should_only_have_two_items_left<GuidItem>();

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
                () => TestContext.Database.should_only_have_two_items_left<IdentityItem>();

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
                () => TestContext.Database.should_only_have_two_items_left<GuidItem>();

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
                () => TestContext.Database.should_only_have_two_items_left<IdentityItem>();

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
                () => TestContext.Database.should_only_have_two_items_left<BigIdentityItem>();

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            private static IList<BigIdentityItem> _structures;
        }

        public static class Establishments
        {
            public static IList<GuidItem> InsertGuidItems(this ISisoDatabase db, int numOfItems)
            {
                var items = new List<GuidItem>(numOfItems);

                for (var c = 0; c < numOfItems; c++)
                    items.Add(new GuidItem { Value = c + 1 });

                using (var uow = db.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }

                return items;
            }

            public static IList<IdentityItem> InsertIdentityItems(this ISisoDatabase db, int numOfItems)
            {
                var items = new List<IdentityItem>(numOfItems);

                for (var c = 0; c < numOfItems; c++)
                    items.Add(new IdentityItem { Value = c + 1 });

                using (var uow = db.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }

                return items;
            }

            public static IList<BigIdentityItem> InsertBigIdentityItems(this ISisoDatabase db, int numOfItems)
            {
                var items = new List<BigIdentityItem>(numOfItems);

                for (var c = 0; c < numOfItems; c++)
                    items.Add(new BigIdentityItem { Value = c + 1 });

                using (var uow = db.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }

                return items;
            }
        }

        public static class Shoulds
        {
            public static It should_only_have_two_items_left<T>(this ISisoDatabase db) where T : class 
            {
                It it = () =>
                {
                    using (var qe = db.CreateQueryEngine())
                        qe.Count<T>().ShouldEqual(2);
                };

                return it;
            }

            public static It should_have_first_and_last_item_left(this ISisoDatabase db, IList<GuidItem> structures) 
            {
                It it = () =>
                {
                    using (var qe = db.CreateQueryEngine())
                    {
                        var items = qe.GetAll<GuidItem>().ToList();
                        items[0].StructureId.ShouldEqual(structures[0].StructureId);
                        items[1].StructureId.ShouldEqual(structures[3].StructureId);
                    }
                };

                return it;
            }

            public static It should_have_first_and_last_item_left(this ISisoDatabase db, IList<IdentityItem> structures)
            {
                It it = () =>
                {
                    using (var qe = db.CreateQueryEngine())
                    {
                        var items = qe.GetAll<IdentityItem>().ToList();
                        items[0].StructureId.ShouldEqual(structures[0].StructureId);
                        items[1].StructureId.ShouldEqual(structures[3].StructureId);
                    }
                };

                return it;
            }

            public static It should_have_first_and_last_item_left(this ISisoDatabase db, IList<BigIdentityItem> structures)
            {
                It it = () =>
                {
                    using (var qe = db.CreateQueryEngine())
                    {
                        var items = qe.GetAll<BigIdentityItem>().ToList();
                        items[0].StructureId.ShouldEqual(structures[0].StructureId);
                        items[1].StructureId.ShouldEqual(structures[3].StructureId);
                    }
                };

                return it;
            }
        }

        public class IdentityItem
        {
            public int StructureId { get; set; }

            public int Value { get; set; }
        }

        public class BigIdentityItem
        {
            public long StructureId { get; set; }

            public int Value { get; set; }
        }

        public class GuidItem
        {
            public Guid StructureId { get; set; }

            public int Value { get; set; }
        }
    }
}