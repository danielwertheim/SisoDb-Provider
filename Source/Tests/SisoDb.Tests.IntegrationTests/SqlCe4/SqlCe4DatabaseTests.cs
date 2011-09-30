using System.IO;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Core.Io;
using SisoDb.SqlCe4;
using SisoDb.SqlCe4.Resources;

namespace SisoDb.Tests.IntegrationTests.SqlCe4
{
    [TestFixture]
    public class SqlCe4DatabaseTests : SqlCe4IntegrationTestBase
    {
        protected override void OnFixtureInitialize()
        {
            var cnInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSqlCe4Temp);
            Database = new SqlCe4Database(cnInfo);
            DbHelper = new SqlCe4DbUtils(cnInfo.ConnectionString.PlainString);
        }

        protected override void OnTestInitialize()
        {
            IoHelper.DeleteIfFileExists(Database.FilePath);
        }

        protected override void OnTestFinalize()
        {
            IoHelper.DeleteIfFileExists(Database.FilePath);
        }

        [Test]
        public void Exists_WhenItDoesNotExist_ReturnsFalse()
        {
            var exists = Database.Exists();

            Assert.IsFalse(exists);
        }

        [Test]
        public void Exists_WhenItDoesExist_ReturnsTrue()
        {
            IoHelper.CreateEmptyFile(Database.FilePath);

            var exists = Database.Exists();

            Assert.IsTrue(exists);
        }

        [Test]
        public void CreateIfNotExists_WhenItDoesNotExist_DatabaseFileGetsCreated()
        {
            Database.CreateIfNotExists();

            Assert.IsTrue(IoHelper.FileExists(Database.FilePath));
        }

        [Test]
        public void CreateIfNotExists_WhenOneExists_IsNotDroppedAndRecreated()
        {
            IoHelper.CreateEmptyFile(Database.FilePath);

            var wasDeleted = false;
            using (var fileWatch = new FileSystemWatcher(Path.GetDirectoryName(Database.FilePath), "*.sdf"))
            {
                fileWatch.Deleted += (s, e) => wasDeleted = true;
                fileWatch.EnableRaisingEvents = true;

                Database.CreateIfNotExists();
            }

            Assert.IsFalse(wasDeleted);
            Assert.IsTrue(IoHelper.FileExists(Database.FilePath));
        }

        [Test]
        public void CreateIfNotExists_WhenItDoesNotExist_DatabaseGetsCreatedAndInitialized()
        {
            Database.CreateIfNotExists();

            AssertInitializedDbExists();
        }

        [Test]
        public void InitializeExisting_WhenDbExists_DatabaseGetsInitialized()
        {
            DbHelper.CreateEmptyDb();

            Database.InitializeExisting();

            AssertInitializedDbExists();
        }

        [Test]
        public void EnsureNewDatabase_WhenOneExists_OneIsRecreated()
        {
            DbHelper.CreateEmptyDb();

            var wasDeleted = false;
            using (var fileWatch = new FileSystemWatcher(Path.GetDirectoryName(Database.FilePath), "*.sdf"))
            {
                fileWatch.Deleted += (s, e) => wasDeleted = true;
                fileWatch.EnableRaisingEvents = true;

                Database.EnsureNewDatabase();
            }

            Assert.IsTrue(wasDeleted);
            AssertInitializedDbExists();
        }

        [Test]
        public void EnsureNewDatabase_WhenNoDbExists_DbGetsCreated()
        {
            IoHelper.DeleteIfFileExists(Database.FilePath);

            Database.EnsureNewDatabase();

            AssertInitializedDbExists();
        }

        [Test]
        public void InitializeExisting_WhenNoDatabaseExists_ThrowsSisoDbException()
        {
            IoHelper.DeleteIfFileExists(Database.FilePath);

            var ex = Assert.Throws<SisoDbException>(() => Database.InitializeExisting());

            Assert.AreEqual(
                SqlCe4Exceptions.SqlCe4Database_InitializeExisting_DbDoesNotExist.Inject(Database.Name),
                ex.Message);
        }

        private void AssertInitializedDbExists()
        {
            var dbExists = Database.Exists();
            Assert.IsTrue(dbExists);

            var identitiesTableExists = DbHelper.TableExists("SisoDbIdentities");
            Assert.IsTrue(identitiesTableExists, "SisoDbIdentities table was not created");
        }
    }
}