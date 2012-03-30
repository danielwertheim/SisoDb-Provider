using System;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class DbObjectNameValidatorTests : UnitTestBase
    {
        private const string ExpectedExceptionMessageFormat = "value '{0}' does not match '^[a-zA-Z_][a-zA-Z0-9_]*$'\r\nParameter name: dbObjectName";

        [Test]
        [TestCase("_")]
        [TestCase("a")]
        [TestCase("A")]
        [TestCase("aAbB_cC_0123456789")]
        public void EnsureValid_DoesNotThrowException_For(string dbObjectName)
        {
            Assert.DoesNotThrow(() => DbObjectNameValidator.EnsureValid(dbObjectName));
        }

        [Test]
        [TestCase("1")]
        [TestCase("a*")]
        [TestCase("a'")]
        [TestCase("a;")]
        [TestCase("a b")]
        public void EnsureValid_ThrowsArgumentException_For(string dbObjectName)
        {
            var ex = Assert.Throws<ArgumentException>(() => DbObjectNameValidator.EnsureValid(dbObjectName));

            Assert.AreEqual("dbObjectName", ex.ParamName);
            Assert.AreEqual(string.Format(ExpectedExceptionMessageFormat, dbObjectName), ex.Message);
        }
    }
}