using System;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.Sql2008;
using SisoDb.Resources;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.StructureSetUpdaterTests
{
    [TestFixture]
    public class SqlStructureSetUpdaterTestsForSameModelNames : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ModelOld.ItemForPropChange>();
            DropStructureSet<ModelNew.ItemForPropChange>();
            DropStructureSet<ModelOld.GuidItemForPropChange>();
            DropStructureSet<ModelNew.GuidItemForPropChange>();
        }

        [Test]
        public void Process_WhenPropertyChangesExists_ValuesCanBeHandled()
        {
            var orgItem = new ModelOld.ItemForPropChange { Int1 = 33, String1 = "Daniel" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.ItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>();
            updater.Process((oldItem, newItem) =>
            {
                Assert.AreEqual(orgItem.SisoId, oldItem.SisoId);
                Assert.AreEqual(33, oldItem.Int1);
                Assert.AreEqual("Daniel", oldItem.String1);

                newItem.NewString1 = oldItem.String1;

                return StructureSetUpdaterStatuses.Keep;
            });

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem = uow.GetById<ModelNew.ItemForPropChange>(orgItem.SisoId);
                Assert.AreEqual("Daniel", newItem.NewString1);
            }
        }

        [Test]
        public void Process_WhenIdentityId_IdIsNotChanged()
        {
            var orgItem = new ModelOld.ItemForPropChange();
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.ItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>();
            updater.Process((oldItem, newItem) => StructureSetUpdaterStatuses.Keep);

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem = uow.GetById<ModelNew.ItemForPropChange>(orgItem.SisoId);
                Assert.IsNotNull(newItem);
            }
        }

        [Test]
        public void Process_WhenGuidId_IdIsNotChanged()
        {
            var orgItem = new ModelOld.GuidItemForPropChange();
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.GuidItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>();
            updater.Process((oldItem, newItem) => StructureSetUpdaterStatuses.Keep);

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem = uow.GetById<ModelNew.GuidItemForPropChange>(orgItem.SisoId);
                Assert.IsNotNull(newItem);
            }
        }

        [Test]
        public void Process_WhenTheNewStructureDoesNotGetAnIdentityId_ThrowsException()
        {
            var orgItem = new ModelOld.ItemForPropChange { Int1 = 33, String1 = "Daniel" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.ItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => updater.Process((oldItem, newItem) =>
                {
                    newItem.SisoId = 0;
                    return StructureSetUpdaterStatuses.Keep;
                }));

            Assert.AreEqual(ExceptionMessages.Id_IdentityIsOutOfRange + "\r\nParameter name: value", ex.Message);
        }

        [Test]
        public void Process_WhenTheNewStructureDoesNotGetAnGuidId_ThrowsException()
        {
            var orgItem = new ModelOld.GuidItemForPropChange { Int1 = 33, String1 = "Daniel" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.GuidItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => updater.Process((oldItem, newItem) =>
                {
                    newItem.SisoId = Guid.Empty;
                    return StructureSetUpdaterStatuses.Keep;
                }));

            Assert.AreEqual(ExceptionMessages.Id_GuidIsMissingValue + "\r\nParameter name: value", ex.Message);
        }

        [Test]
        public void Process_WhenTheNewStructureGainsDifferentIdentityId_ThrowsException()
        {
            var orgItem = new ModelOld.ItemForPropChange { Int1 = 33, String1 = "Daniel" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.ItemForPropChange>.Type);

            var oldIdForAssert = 0;
            var newIdForAssert = 0;

            var updater = CreateUpserterFor<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>();
            var ex = Assert.Throws<SisoDbException>(
                () => updater.Process((oldItem, newItem) =>
                {
                    newItem.SisoId = oldItem.SisoId + 1;

                    oldIdForAssert = oldItem.SisoId;
                    newIdForAssert = newItem.SisoId;

                    return StructureSetUpdaterStatuses.Keep;
                }));

            Assert.AreEqual(ExceptionMessages.SqlStructureSetUpdater_NewIdDoesNotMatchOldId
                .Inject(newIdForAssert, oldIdForAssert), ex.Message);
        }

        [Test]
        public void Process_WhenTheNewStructureGainsDifferentGuidId_ThrowsException()
        {
            var oldIdForAssert = Guid.Empty;
            var newIdForAssert = new Guid("EBFF8295-AFC0-4734-A4DD-6CEC95053D9D");

            var orgItem = new ModelOld.GuidItemForPropChange { Int1 = 33, String1 = "Daniel" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.ItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>();
            var ex = Assert.Throws<SisoDbException>(
                () => updater.Process((oldItem, newItem) =>
                                          {
                                              oldIdForAssert = oldItem.SisoId;
                                              newItem.SisoId = newIdForAssert;

                                              return StructureSetUpdaterStatuses.Keep;
                                          }));

            Assert.AreEqual(ExceptionMessages.SqlStructureSetUpdater_NewIdDoesNotMatchOldId
                                .Inject(newIdForAssert, oldIdForAssert), ex.Message);
        }

        [Test]
        public void Process_WhenTwoStructuresExistsAndTrashIsMadeOnSecond_OnlyTheFirstItemRemains()
        {
            var orgItem1 = new ModelOld.GuidItemForPropChange { Int1 = 10 };
            var orgItem2 = new ModelOld.GuidItemForPropChange { Int1 = 20 };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(new[] { orgItem1, orgItem2 });
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.GuidItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>();
            updater.Process((oldItem, newItem)
                => oldItem.SisoId.Equals(orgItem2.SisoId) ? StructureSetUpdaterStatuses.Trash : StructureSetUpdaterStatuses.Keep);

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem1 = uow.GetById<ModelNew.GuidItemForPropChange>(orgItem1.SisoId);
                Assert.IsNotNull(newItem1);

                var newItem2 = uow.GetById<ModelNew.GuidItemForPropChange>(orgItem2.SisoId);
                Assert.IsNull(newItem2);
            }
        }

        [Test]
        public void Process_WhenThreeStructuresExistsAndTrashIsMadeOnSecond_OnlyFirstAndThirdItemsRemains()
        {
            var orgItem1 = new ModelOld.GuidItemForPropChange { String1 = "A" };
            var orgItem2 = new ModelOld.GuidItemForPropChange { String1 = "B" };
            var orgItem3 = new ModelOld.GuidItemForPropChange { String1 = "C" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.GuidItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>();
            updater.Process((oldItem, newItem) =>
            {
                newItem.NewString1 = oldItem.String1;
                return oldItem.SisoId.Equals(orgItem2.SisoId)
                            ? StructureSetUpdaterStatuses.Trash
                            : StructureSetUpdaterStatuses.Keep;
            });

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem1 = uow.GetById<ModelNew.GuidItemForPropChange>(orgItem1.SisoId);
                Assert.IsNotNull(newItem1);
                Assert.AreEqual("A", newItem1.NewString1);

                var newItem2 = uow.GetById<ModelNew.GuidItemForPropChange>(orgItem2.SisoId);
                Assert.IsNull(newItem2);

                var newItem3 = uow.GetById<ModelNew.GuidItemForPropChange>(orgItem3.SisoId);
                Assert.IsNotNull(newItem3);
                Assert.AreEqual("C", newItem3.NewString1);
            }
        }

        [Test]
        public void Process_WhenThreeStructuresWithIdentitiesExistsAndTrashIsMadeOnSecond_OnlyFirstAndThirdItemsRemains()
        {
            var orgItem1 = new ModelOld.ItemForPropChange { String1 = "A" };
            var orgItem2 = new ModelOld.ItemForPropChange { String1 = "B" };
            var orgItem3 = new ModelOld.ItemForPropChange { String1 = "C" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.ItemForPropChange>.Type);

            var id1 = orgItem1.SisoId;
            var id2 = orgItem2.SisoId;
            var id3 = orgItem3.SisoId;

            var updater = CreateUpserterFor<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>();
            updater.Process((oldItem, newItem) =>
            {
                newItem.NewString1 = oldItem.String1;
                return oldItem.SisoId.Equals(id2)
                            ? StructureSetUpdaterStatuses.Trash
                            : StructureSetUpdaterStatuses.Keep;
            });

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem1 = uow.GetById<ModelNew.ItemForPropChange>(id1);
                Assert.IsNotNull(newItem1);
                Assert.AreEqual("A", newItem1.NewString1);

                var newItem2 = uow.GetById<ModelNew.ItemForPropChange>(id2);
                Assert.IsNull(newItem2);

                var newItem3 = uow.GetById<ModelNew.ItemForPropChange>(id3);
                Assert.IsNotNull(newItem3);
                Assert.AreEqual("C", newItem3.NewString1);
            }
        }

        [Test]
        public void Process_WhenTwoStructuresExistsAndAbortIsMadeOnSecond_StructureCanBeReadBack()
        {
            var orgItem1 = new ModelOld.GuidItemForPropChange { Int1 = 10, String1 = "Arbitrary string1" };
            var orgItem2 = new ModelOld.GuidItemForPropChange { Int1 = 20, String1 = "Arbitrary string2" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(new[] { orgItem1, orgItem2 });
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ModelOld.GuidItemForPropChange>.Type);

            var updater = CreateUpserterFor<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>();
            updater.Process((oldItem, newItem)
                            => oldItem.SisoId.Equals(orgItem1.SisoId) ? StructureSetUpdaterStatuses.Keep : StructureSetUpdaterStatuses.Abort);

            Database.StructureSchemas.RemoveSchema(TypeFor<ModelNew.GuidItemForPropChange>.Type);
            using (var uow = Database.CreateUnitOfWork())
            {
                var refetchedOrgItem1 = uow.GetById<ModelOld.GuidItemForPropChange>(orgItem1.SisoId);
                Assert.IsNotNull(refetchedOrgItem1);
                Assert.AreEqual(10, refetchedOrgItem1.Int1);
                Assert.AreEqual("Arbitrary string1", refetchedOrgItem1.String1);

                var refetchedOrgItem2 = uow.GetById<ModelOld.GuidItemForPropChange>(orgItem2.SisoId);
                Assert.IsNotNull(refetchedOrgItem2);
                Assert.AreEqual(20, refetchedOrgItem2.Int1);
                Assert.AreEqual("Arbitrary string2", refetchedOrgItem2.String1);
            }
        }

        private SqlStructureSetUpdater<TOld, TNew> CreateUpserterFor<TOld, TNew>()
            where TOld : class
            where TNew : class
        {
            var structureSchemaOld = Database.StructureSchemas.GetSchema(TypeFor<TOld>.Type);
            Database.StructureSchemas.RemoveSchema(TypeFor<TOld>.Type);

            var structureSchemaNew = Database.StructureSchemas.GetSchema(TypeFor<TNew>.Type);
            var updater = new SqlStructureSetUpdater<TOld, TNew>(
                Database.ConnectionInfo, structureSchemaOld, structureSchemaNew, Database.StructureBuilder);

            return updater;
        }
    }

    namespace ModelOld
    {
        public class ItemForPropChange
        {
            public int SisoId { get; set; }

            public string String1 { get; set; }

            public int Int1 { get; set; }
        }

        public class GuidItemForPropChange
        {
            public Guid SisoId { get; set; }

            public string String1 { get; set; }

            public int Int1 { get; set; }
        }
    }

    namespace ModelNew
    {
        public class ItemForPropChange
        {
            public int SisoId { get; set; }

            public string NewString1 { get; set; }
        }

        public class GuidItemForPropChange
        {
            public Guid SisoId { get; set; }

            public string NewString1 { get; set; }
        }
    }
}