using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterEnumerableIndexablePropertiesTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void GetIndexableProperties_WhenIListOfTIndexesExists_ReturnsTheElementMembers()
        {
            var props = ReflecterFor().GetIndexableProperties(typeof(WithCollectionIndexes), false);

            Assert.IsTrue(props.Any(p => p.Path == "IList1.ElementInt1"));
        }

        [Test]
        public void GetIndexableProperties_WhenIEnumerableOfTIndexesExists_ReturnsTheElementMembers()
        {
            var props = ReflecterFor().GetIndexableProperties(typeof(WithCollectionIndexes), false);

            Assert.IsTrue(props.Any(p => p.Path == "IEnumerable1.ElementInt1"));
        }

        [Test]
        public void GetIndexableProperties_WhenEnumerableOfBytes_NoPropertiesAreReturned()
        {
            var props = ReflecterFor().GetIndexableProperties(typeof(WithEnumerableBytes), false);

            Assert.IsFalse(props.Any());
        }

        [Test]
        public void GetIndexableProperties_WhenArrayOfStrings_OnlyReturnsPropertyForAccessingTheStringArray()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithArrayOfStrings), false);
            var arrayIndex = properties.SingleOrDefault(p => p.Path == "Values");

            Assert.AreEqual(1, properties.Count());
            Assert.IsNotNull(arrayIndex);
        }

        [Test]
        public void GetIndexableProperties_WhenArrayOfIntegers_OnlyReturnsPropertyForAccessingTheStringArray()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithArrayOfIntegers), false);
            var arrayIndex = properties.SingleOrDefault(p => p.Path == "Values");

            Assert.AreEqual(1, properties.Count());
            Assert.IsNotNull(arrayIndex);
        }

        [Test]
        public void GetIndexableProperties_WhenWithNestedArrayOfStrings_OnlyReturnsPropertyForAccessingTheStringArray()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithNestedArrayOfStrings), false);
            var arrayIndex = properties.SingleOrDefault(p => p.Path == "Item.Values");

            Assert.AreEqual(1, properties.Count());
            Assert.IsNotNull(arrayIndex);
        }

        [Test]
        public void GetIndexableProperties_WhenWithArrayOfNestedArrayOfStrings_OnlyReturnsPropertyForAccessingTheStringArray()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithArrayOfNestedArrayOfStrings), false);
            var arrayIndex = properties.SingleOrDefault(p => p.Path == "Items.Values");

            Assert.AreEqual(1, properties.Count());
            Assert.IsNotNull(arrayIndex);
        }

        private class WithEnumerableBytes
        {
            public byte[] Bytes1 { get; set; }
            public IEnumerable<byte> Bytes2 { get; set; }
            public IList<byte> Bytes3 { get; set; }
            public List<byte> Bytes4 { get; set; }
            public ICollection<byte> Bytes5 { get; set; }
            public Collection<byte> Bytes6 { get; set; }
        }

        private class WithCollectionIndexes
        {
            public IEnumerable<Element> IEnumerable1 { get; set; }
            public IList<Element> IList1 { get; set; }
        }

        private class Element
        {
            public int ElementInt1 { get; set; }
        }

        private class WithArrayOfStrings
        {
            public string[] Values { get; set; }
        }

        private class WithArrayOfIntegers
        {
            public int[] Values { get; set; }
        }

        private class WithNestedArrayOfStrings
        {
            public WithArrayOfStrings Item { get; set; }
        }

        private class WithArrayOfNestedArrayOfStrings
        {
            public WithArrayOfStrings[] Items { get; set; }
        }
    }
}