using System;
using NUnit.Framework;

namespace SisoDb.Tests.UnitTests
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

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CnString_IsNull_ThrowsArgumentNullException()
        {
            new ConnectionString(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CnString_IsWhiteSpaced_ThrowsArgumentNullException()
        {
            new ConnectionString(" ");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void CnString_HasNoDelim_ThrowsArgumentNullException()
        {
            new ConnectionString("Arbitrary string");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage =
            "The connectionstring should have exactly two parts ('sisodb:' and 'plain:')." +
            " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.")]
        public void CnString_HasDelim_ThrowsArgumentException()
        {
            new ConnectionString("||");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage =
            "The connectionstring should have exactly two parts ('sisodb:' and 'plain:')." +
            " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.")]
        public void CnString_HasDelimWithLeftValue_ThrowsArgumentException()
        {
            new ConnectionString("A||");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage =
            "The connectionstring should have exactly two parts ('sisodb:' and 'plain:')." +
            " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.")]
        public void CnString_HasDelimWithRightValue_ThrowsArgumentException()
        {
            new ConnectionString("||A");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage =
            "The connectionstring is missing the SisoDb-part, indicated by 'sisodb:'." +
            " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.")]
        public void CnString_HasTwoPartsButMissesSisoDbMarker_ThrowsArgmentException()
        {
            new ConnectionString("A||plain:x");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage =
            "The connectionstring is missing the Plain-part, indicated by 'plain:'." +
            " Example: 'sisodb:[SisoDb configvalues];||plain:[Plain configvalues]'.")]
        public void CnString_HasTwoPartsButMissesPlainMarker_ThrowsArgmentException()
        {
            new ConnectionString("sisodb:A||x");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage =
            "The SisoDb-part is missing required key: 'provider'.")]
        public void CnString_SisoDbPartMissesStorageProviderKey_ThrowsAgumentException()
        {
            new ConnectionString("sisodb:k1=v1||plain:x");
        }
    }
}