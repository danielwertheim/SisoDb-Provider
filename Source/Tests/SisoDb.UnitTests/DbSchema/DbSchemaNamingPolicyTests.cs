using System;
using NUnit.Framework;
using SisoDb.DbSchema;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class DbSchemaNamingPolicyTests : UnitTestBase
    {
        [Test]
        public void StructureNamePrefix_does_not_accept_prefix_longer_than_eight_chars()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => DbSchemaNamingPolicy.StructureNamePrefix = new string('a', DbSchemaNamingPolicy.MaxLenOfPrefix + 1));

            Assert.AreEqual(ex.Message, ExceptionMessages.DbSchemaNamingPolicy_StructureNamePrefix_IsToLong.Inject(DbSchemaNamingPolicy.MaxLenOfPrefix));
        }
    }
}