using System;
using NUnit.Framework;
using SisoDb.Cryptography;

namespace SisoDb.Tests.UnitTests.Cryptography
{
    [TestFixture]
    public class HashServiceTests
    {
        [Test]
        public void GetHashLength_Returns8()
        {
            var hasher = new HashService();
            var hashLength = hasher.GetHashLength();

            Assert.AreEqual(8, hashLength);
        }

        [Test]
        public void HashLengthComparision_WhenStringEmpty_AreEqual()
        {
            var hasher = new HashService();
            var hashLength = hasher.GetHashLength();

            var hash = hasher.GenerateHash(string.Empty);

            Assert.AreEqual(hashLength, hash.Length);
        }

        [Test]
        public void HashLengthComparision_WhenNotStringEmpty_AreEqual()
        {
            var hasher = new HashService();
            var hashLength = hasher.GetHashLength();

            var hash = hasher.GenerateHash("Some arbitrary text.");

            Assert.AreEqual(hashLength, hash.Length);
        }

        [Test]
        public void GenerateHash_WhenNullInput_ThrowsArgumentNullException()
        {
            var hasher = new HashService();

            Assert.Throws<ArgumentNullException>(() => hasher.GenerateHash(null));
        }

        [Test]
        public void GenerateHash_EmptyString_Returns8Zeros()
        {
            var value = string.Empty;

            var hasher = new HashService();
            var hash = hasher.GenerateHash(value);

            Assert.AreEqual("00000000", hash);
        }

        [Test]
        public void GenerateHash_When256Chars_Returns8Chars()
        {
            var value = new string('a', 256);
            
            var hasher = new HashService();
            var hash = hasher.GenerateHash(value);

            Assert.AreEqual("b07d3659", hash);
        }

        [Test]
        public void GenerateHash_When512Chars_Returns8Chars()
        {
            var value = new string('a', 512);

            var hasher = new HashService();
            var hash = hasher.GenerateHash(value);

            Assert.AreEqual("f9517051", hash);
        }

        [Test]
        public void GenerateHash_When1024Chars_Returns8Chars()
        {
            var value = new string('a', 1024);

            var hasher = new HashService();
            var hash = hasher.GenerateHash(value);

            Assert.AreEqual("7c5597b9", hash);
        }

        [Test]
        public void GenerateHash_When2048Chars_Returns8Chars()
        {
            var value = new string('a', 2048);

            var hasher = new HashService();
            var hash = hasher.GenerateHash(value);

            Assert.AreEqual("443d72ec", hash);
        }
    }
}