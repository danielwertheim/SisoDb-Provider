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