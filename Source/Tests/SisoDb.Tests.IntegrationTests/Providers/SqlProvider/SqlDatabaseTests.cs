using System;
using System.Data;
using NUnit.Framework;
using SisoDb.Providers.SqlProvider;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider
{
    [TestFixture]
    public class SqlDatabaseTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DbHelper.DropDatabase(LocalConstants.TempDbName);
            Database.DropStructureSet<ItemForUpsertStructureSet>();
            Database.DropStructureSet<ModelOld.ItemForPropChange>();
            Database.DropStructureSet<ModelNew.ItemForPropChange>();
        }

        [Test]
        public void Exists_WhenItExists_ReturnsTrue()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForTemp);
            
            var db = new SqlDatabase(connectionInfo);
            var dbExists = db.Exists();

            Assert.IsTrue(dbExists);
        }

        [Test]
        public void Exists_WhenItDoesNotExist_ReturnsTrue()
        {
            DbHelper.DropDatabase(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForTemp);
            var db = new SqlDatabase(connectionInfo);

            var dbExists = db.Exists();

            Assert.IsFalse(dbExists);
        }

        [Test]
        public void CreateIfNotExists_WhenNoDatabaseExists_DatabaseGetsCreated()
        {
            DbHelper.DropDatabase(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForTemp);

            var db = new SqlDatabase(connectionInfo);
            db.CreateIfNotExists();

            var dbExists = DbHelper.DatabaseExists(LocalConstants.TempDbName);
            Assert.IsTrue(dbExists);
        }

        [Test]
        public void DeleteIfExists_WhenDatabaseExists_DatabaseGetsDropped()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForTemp);

            var db = new SqlDatabase(connectionInfo);
            db.DeleteIfExists();

            var dbExists = db.Exists();
            Assert.IsFalse(dbExists);
        }

        [Test]
        public void InitializeExisting_WhenDatabaseDoesNotExist_ThrowsSisoDbException()
        {
            DbHelper.DropDatabase(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForTemp);

            var db = new SqlDatabase(connectionInfo);

            Assert.Throws<SisoDbException>(() => db.InitializeExisting());
        }

        [Test]
        public void InitializeExisting_WhenDatabaseExists_CreatesSisoSysTables()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForTemp);

            var db = new SqlDatabase(connectionInfo);
            db.InitializeExisting();

            var identitiesTableExists = DbHelper.TableExists("SisoDbIdentities");
            Assert.IsTrue(identitiesTableExists);
        }

        [Test]
        public void UpsertStructureSet_WhenEntityHasGuidId_IdColumnGetsRowGuidCol()
        {
            Database.UpsertStructureSet<ItemForUpsertStructureSet>();

            var structureTableHasRowGuidCol = DbHelper.ExecuteScalar<bool>(CommandType.Text,
                "select columnproperty(object_id('ItemForUpsertStructureSetStructure'), 'Id', 'IsRowGuidCol');");
            var indexesTableHasRowGuidCol = DbHelper.ExecuteScalar<bool>(CommandType.Text,
                "select columnproperty(object_id('ItemForUpsertStructureSetIndexes'), 'StructureId', 'IsRowGuidCol');");

            Assert.IsTrue(structureTableHasRowGuidCol, "Structure table");
            Assert.IsTrue(indexesTableHasRowGuidCol, "Indexes table");
        }

        [Test]
        public void UpdateStructureSet_WhenThreeStructuresWithIdentitiesExistsAndTrashIsMadeOnSecond_OnlyFirstAndThirdItemsRemains()
        {
            var orgItem1 = new ModelOld.ItemForPropChange { String1 = "A" };
            var orgItem2 = new ModelOld.ItemForPropChange { String1 = "B" };
            var orgItem3 = new ModelOld.ItemForPropChange { String1 = "C" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema<ModelOld.ItemForPropChange>();

            var id1 = orgItem1.Id;
            var id2 = orgItem2.Id;
            var id3 = orgItem3.Id;
            Database.UpdateStructureSet<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>(
            (oldItem, newItem) =>
            {
                newItem.NewString1 = oldItem.String1;
                return oldItem.Id.Equals(id2)
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

        private class ItemForUpsertStructureSet
        {
            public Guid Id { get; set; }

            public string Temp
            {
                get { return "Some text to get rid of exception of no indexable members."; }
            }
        }
    }
}