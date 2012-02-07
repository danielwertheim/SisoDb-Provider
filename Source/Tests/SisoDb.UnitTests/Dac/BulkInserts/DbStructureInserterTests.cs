using System;
using Moq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;

namespace SisoDb.UnitTests.Dac.BulkInserts
{
    [TestFixture]
    public class DbStructureInserterTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenNoDbClientFnIsPassedAndConnectionInfoSupportsParallelInserts_StructureInserterWillNotSupportParallelInserts()
        {
            var cnInfoFake = new Mock<ISisoConnectionInfo>();
            cnInfoFake.Setup(f => f.ParallelInsertMode).Returns(ParallelInsertMode.Full);
            var dbClientFake = new Mock<IDbClient>();
            dbClientFake.Setup(f => f.ConnectionInfo).Returns(cnInfoFake.Object);

            var inserter = new DbStructureInserterExtendedForTest(dbClientFake.Object, null);
            
            Assert.IsFalse(inserter.GetSupportsParallelInserts());
        }

        private class DbStructureInserterExtendedForTest : DbStructureInserter
        {
            public DbStructureInserterExtendedForTest(IDbClient mainDbClient, Func<IDbClient> dbClientFnForParallelInserts = null) 
                : base(mainDbClient, dbClientFnForParallelInserts) {}

            public bool GetSupportsParallelInserts()
            {
                return base.SupportsParallelInserts;
            }
        }
    }
}