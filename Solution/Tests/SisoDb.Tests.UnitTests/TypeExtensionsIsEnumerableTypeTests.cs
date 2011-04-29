using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class TypeExtensionsIsEnumerableTypeTests : UnitTestBase
    {
        [Test]
        public void IsEnumerableType_WhenIEnumerable_ReturnsTrue()
        {
            Assert.IsTrue(typeof(IEnumerable<int>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenNullableIEnumearble_ReturnsTrue()
        {
            Assert.IsTrue(typeof(IEnumerable<int?>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenIList_ReturnsTrue()
        {
            Assert.IsTrue(typeof(IList<int>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenNullableIList_ReturnsTrue()
        {
            Assert.IsTrue(typeof(IList<int?>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenList_ReturnsTrue()
        {
            Assert.IsTrue(typeof(List<int>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenNullablList_ReturnsTrue()
        {
            Assert.IsTrue(typeof(List<int?>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenISet_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ISet<int>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenNullableISet_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ISet<int?>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenHashSet_ReturnsTrue()
        {
            Assert.IsTrue(typeof(HashSet<int>).IsEnumerableType());
        }

        [Test]
        public void IIsEnumerableType_WhenNullableHashSet_ReturnsTrue()
        {
            Assert.IsTrue(typeof(HashSet<int?>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenICollection_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ICollection<int>).IsEnumerableType());
        }

        [Test]
        public void IIsEnumerableType_WhenNullableICollection_ReturnsTrue()
        {
            Assert.IsTrue(typeof(ICollection<int?>).IsEnumerableType());
        }

        [Test]
        public void IsEnumerableType_WhenCollection_ReturnsTrue()
        {
            Assert.IsTrue(typeof(Collection<int>).IsEnumerableType());
        }

        [Test]
        public void IIsEnumerableType_WhenNullableCollection_ReturnsTrue()
        {
            Assert.IsTrue(typeof(Collection<int?>).IsEnumerableType());
        }
    }
}