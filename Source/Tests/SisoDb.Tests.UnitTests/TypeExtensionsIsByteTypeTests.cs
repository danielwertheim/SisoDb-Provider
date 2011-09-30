using NUnit.Framework;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class TypeExtensionsIsByteTypeTests : UnitTestBase
    {
        [Test]
        public void IsByteType_WhenByte_ReturnsTrue()
        {
            Assert.IsTrue(typeof(byte).IsByteType());
        }

        [Test]
        public void IsByteType_WhenInt_ReturnsFalse()
        {
            Assert.IsFalse(typeof(int).IsByteType());
        }

        [Test]
        public void IsNullableByteType_WhenNullableByte_ReturnsTrue()
        {
            Assert.IsTrue(typeof(byte?).IsNullableByteType());
        }

        [Test]
        public void IsNullableByteType_WhenNullableInt_ReturnsFalse()
        {
            Assert.IsFalse(typeof(int?).IsNullableByteType());
        }
    }
}