using System.IO;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Core.Io;
using SisoDb.Providers.SqlCe4;
using SisoDb.Providers.SqlCe4.Resources;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlCe4
{
    [TestFixture]
    public class SqlCe4DatabaseTests : SqlCe4IntegrationTestBase
    {
        private readonly SqlCe4Database _dbForTemp;

        public SqlCe4DatabaseTests()
        {
            var cnInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSqlCe4Temp);
            _dbForTemp = new SqlCe4Database(cnInfo);
        }

        protected override void OnTestFinalize()
        {
            IoHelper.DeleteIfFileExists(_dbForTemp.FilePath);
        }

        [Test]
        public void CreateIfNotExists_WhenItDoesNotExist_DatabaseGetsCreated()
        {
            IoHelper.DeleteIfFileExists(_dbForTemp.FilePath);

            Assert.IsFalse(IoHelper.FileExists(_dbForTemp.FilePath));

            _dbForTemp.CreateIfNotExists();

            Assert.IsTrue(IoHelper.FileExists(_dbForTemp.FilePath));
        }

        [Test]
        public void CreateIfNotExists_WhenOneExists_IsNotDroppedAndRecreated()
        {
            _dbForTemp.CreateIfNotExists();

            var wasDeleted = false;
            using (var fileWatch = new FileSystemWatcher(Path.GetDirectoryName(_dbForTemp.FilePath), "*.sdf"))
            {
                fileWatch.Deleted += (s, e) => wasDeleted = true;
                fileWatch.EnableRaisingEvents = true;

                _dbForTemp.CreateIfNotExists();
            }

            Assert.IsFalse(wasDeleted);
            Assert.IsTrue(IoHelper.FileExists(_dbForTemp.FilePath));
        }

        [Test]
        public void EnsureNewDatabase_WhenOneExists_OneIsRecreated()
        {
            _dbForTemp.CreateIfNotExists();

            var wasDeleted = false;
            using (var fileWatch = new FileSystemWatcher(Path.GetDirectoryName(_dbForTemp.FilePath), "*.sdf"))
            {
                fileWatch.Deleted += (s, e) => wasDeleted = true;
                fileWatch.EnableRaisingEvents = true;

                _dbForTemp.EnsureNewDatabase();
            }

            Assert.IsTrue(wasDeleted);
            Assert.IsTrue(IoHelper.FileExists(_dbForTemp.FilePath));
        }

        [Test]
        public void EnsureNewDatabase_WhenNoneExists_GetsCreated()
        {
            IoHelper.DeleteIfFileExists(_dbForTemp.FilePath);

            _dbForTemp.EnsureNewDatabase();

            Assert.IsTrue(IoHelper.FileExists(_dbForTemp.FilePath));
        }

        //[Test]
        //public void EnsureNewDatabase_SystemTablesWillGetCreated()
        //{
        //    Assert.Fail("Implement");
        //}

        //[Test]
        //public void EnsureNewDatabase_SystemTypesWillGetCreated()
        //{
        //    Assert.Fail("Implement");
        //}

        [Test]
        public void InitializeExisting_WhenNoDatabaseExists_ThrowsSisoDbException()
        {
            IoHelper.DeleteIfFileExists(_dbForTemp.FilePath);

            var ex = Assert.Throws<SisoDbException>(() => _dbForTemp.InitializeExisting());

            Assert.AreEqual(
                SqlCe4Exceptions.SqlCe4Database_InitializeExisting_DbDoesNotExist.Inject(_dbForTemp.Name),
                ex.Message);
        }
    }
}