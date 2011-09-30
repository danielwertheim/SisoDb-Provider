using System;
using System.Collections;
using NUnit.Framework;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class TypeExtensionsIsSimpleTypeTests : UnitTestBase
    {
        [Test]
        public void IsSimpleType_WhenClass_ReturnsFalse()
        {
            Assert.IsFalse(typeof(DummyClass).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenInt_ReturnsTrue()
        {
            Assert.IsTrue(typeof(int).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenByte_ReturnsTrue()
        {
            Assert.IsTrue(typeof(byte).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenDecimal_ReturnsTrue()
        {
            Assert.IsTrue(typeof(decimal).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenDateTime_ReturnsTrue()
        {
            Assert.IsTrue(typeof(DateTime).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenString_ReturnsTrue()
        {
            Assert.IsTrue(typeof(string).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenBool_ReturnsTrue()
        {
            Assert.IsTrue(typeof(bool).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenDouble_ReturnsTrue()
        {
            Assert.IsTrue(typeof(double).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenFloat_ReturnsTrue()
        {
            Assert.IsTrue(typeof(float).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenGuid_ReturnsTrue()
        {
            Assert.IsTrue(typeof(Guid).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenEnum_ReturnsTrue()
        {
            Assert.IsTrue(typeof(DummyEnum).IsSimpleType());
        }

        [Test]
        public void IsSimpleType_WhenIEnumerable_ReturnsFalse()
        {
            Assert.IsFalse(typeof(IEnumerable).IsSimpleType());
        }

        [Test]
        public void IsNullableValueType_WhenNullableInt_ReturnsTrue()
        {
            Assert.IsTrue(typeof(int?).IsNullableValueType());
        }

        [Test]
        public void IsNullableValueType_WhenInt_ReturnsFalse()
        {
            Assert.IsFalse(typeof(int).IsNullableValueType());
        }

        [Test]
        public void IsNullableValueType_WhenString_ReturnsFalse()
        {
            Assert.IsFalse(typeof(string).IsNullableValueType());
        }

        private enum DummyEnum
        {
            A,
            B
        }

        private class DummyClass
        {
        }
    }
}