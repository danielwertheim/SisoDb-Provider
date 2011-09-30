using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class TypeExtensionsEnumerableTests : UnitTestBase
    {
        [Test]
        public void IsEnumerableType_WhenISetOfT_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ISet<DummyClass>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenHashSetOfT_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ISet<HashSet<DummyClass>>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenICollectionOfT_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ICollection<DummyClass>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenCollectionOfT_ReturnsTrue()
        {
            Assert.IsTrue(typeof(Collection<DummyClass>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIEnumerable_ReturnsTrue()
        {
            Assert.IsTrue(typeof(IEnumerable<DummyClass>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIListOfT_ReturnsTrue()
        {
            Assert.IsTrue(typeof(IList<DummyClass>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenListOfT_ReturnsTrue()
        {
            Assert.IsTrue(typeof(List<DummyClass>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIntArray_ReturnsTrue()
        {
            Assert.IsTrue(typeof(int[]).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenByteArray_ReturnsTrue()
        {
            Assert.IsTrue(typeof(byte[]).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIEnumerable_ReturnsFalse()
        {
            Assert.IsFalse(typeof(IEnumerable).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIList_ReturnsFalse()
        {
            Assert.IsFalse(typeof(IList).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenICollection_ReturnsFalse()
        {
            Assert.IsFalse(typeof(ICollection).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenArray_ReturnsFalse()
        {
            Assert.IsFalse(typeof(Array).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIDictionary_ReturnsFalse()
        {
            Assert.IsFalse(typeof(IDictionary).IsEnumerableType());
        }

        [Test]
        public void GetEnumerableElementType_WhenIEnumerableOfT_ReturnsElementType()
        {
            var elementType = typeof(IEnumerable<DummyClass>).GetEnumerableElementType();

            Assert.AreEqual(typeof(DummyClass), elementType);
        }

        [Test]
        public void GetEnumerableElementType_WhenIListOfT_ReturnsElementType()
        {
            var elementType = typeof(IList<DummyClass>).GetEnumerableElementType();

            Assert.AreEqual(typeof(DummyClass), elementType);
        }

        [Test]
        public void GetEnumerableElementType_WhenIntArray_ReturnsElementType()
        {
            var elementType = typeof(int[]).GetEnumerableElementType();

            Assert.AreEqual(typeof(int), elementType);
        }

        [Test]
        public void GetEnumerableElementType_WhenCustomList_ReturnsElementType()
        {
            var elementType = typeof(IValidCustomList<int>).GetEnumerableElementType();

            Assert.AreEqual(typeof(int), elementType);
        }

        [Test]
        public void GetEnumerableElementType_WhenMoreThanOneGenericArg_ThrowsSisoDbException()
        {
            var ex = CustomAssert.Throws<SisoDbException>(() => typeof(IInvalidCustomList<int, string>).GetEnumerableElementType());

            Assert.AreEqual(ExceptionMessages.TypeExtensions_ExtractGenericType, ex.Message);
        }

        private class DummyClass
        {
        }

        private interface IValidCustomList<out T1> : IEnumerable<T1>
        { }

        private interface IInvalidCustomList<out T1, T2> : IEnumerable<T1>
        { }
    }
}