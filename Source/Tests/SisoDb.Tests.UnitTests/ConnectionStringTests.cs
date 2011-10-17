using System;
using NUnit.Framework;

namespace SisoDb.UnitTests
{
    [TestFixture]
    public class ConnectionStringTests : UnitTestBase
    {
        private const string ValidCnString = "sisodb:provider=A provider||plain:TestKey1=TestValue1;TestKey2=TestValue2";

        [Test]
        public void SisoDbString_WhenCnStringIsValid_EqualToSisoDbPart()
        {
            var cnString = new ConnectionString(ValidCnString);

            Assert.AreEqual("provider=A provider", cnString.SisoDbString);
        }

        [Test]
        public void PlainString_WhenCnStringIsValid_EqualToPlainPart()
        {
            var cnString = new ConnectionString(ValidCnString);

            Assert.AreEqual("TestKey1=TestValue1;TestKey2=TestValue2", cnString.PlainString);
        }

        [Test]
        public void StorageProvider_WhenSisoDbPartContainsProvider_EqualToPassedValue()
        {
            var cnString = new ConnectionString(ValidCnString);

            Assert.AreEqual("A provider", cnString.Provider);
        }

        [Test]
        public void CnString_IsNull_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString(null));

            Assert.AreEqual("value", ex.ParamName);
        }

        [Test]
        public void CnString_IsWhiteSpaced_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString(" "));

            Assert.AreEqual("value", ex.ParamName);
        }

        [Test]
        public void CnString_HasNoDelim_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("arbitrary string"));

            Assert.AreEqual(
                "The connectionstring should have exactly two parts ('sisodb:' and 'plain:'). " + 
                "Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.", ex.Message);
        }

        [Test]
        public void CnString_HasDelim_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("||"));

            Assert.AreEqual(
                "The connectionstring should have exactly two parts ('sisodb:' and 'plain:')." +
                " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.", ex.Message);
        }

        [Test]
        public void CnString_HasDelimWithLeftValue_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("A||"));

            Assert.AreEqual(
                "The connectionstring should have exactly two parts ('sisodb:' and 'plain:')." +
                " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.", ex.Message);
        }

        [Test]
        public void CnString_HasDelimWithRightValue_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("||A"));

            Assert.AreEqual(
                "The connectionstring should have exactly two parts ('sisodb:' and 'plain:')." +
                " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.", ex.Message);
        }

        [Test]
        public void CnString_HasTwoPartsButMissesSisoDbMarker_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("A||plain:x"));

            Assert.AreEqual(
                "The connectionstring is missing the SisoDb-part, indicated by 'sisodb:'." +
                " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.", ex.Message);
        }

        [Test]
        public void CnString_HasTwoPartsButMissesPlainMarker_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("sisodb:A||x"));

            Assert.AreEqual(
                "The connectionstring is missing the Plain-part, indicated by 'plain:'." +
                " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.", ex.Message);
        }

        [Test]
        public void CnString_SisoDbPartMissesStorageProviderKey_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ConnectionString("sisodb:k1=v1||plain:x"));

            Assert.AreEqual("The SisoDb-part is missing required key: 'provider'.", ex.Message);
        }
    }
}