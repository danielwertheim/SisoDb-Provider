using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterComplexIndexablePropertiesTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void GetIndexableProperties_WhenItemWithComplexProperty_ReturnsComplexProperties()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithComplexProperty), false);

            Assert.AreEqual(2, properties.Count());
            Assert.IsTrue(properties.Any(p => p.Path == "Complex.IntValue"));
            Assert.IsTrue(properties.Any(p => p.Path == "Complex.StringValue"));
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithUniqeAndNonUniqueComplexProperties_ReturnsBothComplexUniqueAndNonUniqueProperties()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithUniqueAndNonUniqueComplexProperties), false);

            Assert.AreEqual(4, properties.Count());
            Assert.IsTrue(properties.Any(p => p.Path == "UqComplex.IntValue"));
            Assert.IsTrue(properties.Any(p => p.Path == "UqComplex.StringValue"));
            Assert.IsTrue(properties.Any(p => p.Path == "NonUqComplex.IntValue"));
            Assert.IsTrue(properties.Any(p => p.Path == "NonUqComplex.StringValue"));
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithEnumerable_EnumerableMemberIsNotReturnedAsComplex()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithEnumerableOfComplex), false);

            Assert.AreEqual(2, properties.Count());
            Assert.IsTrue(properties.Any(p => p.Path == "Items.IntValue"));
            Assert.IsTrue(properties.Any(p => p.Path == "Items.StringValue"));
        }
        
        private class WithComplexProperty
        {
            public Item Complex { get; set; }
        }

        private class WithUniqueAndNonUniqueComplexProperties
        {
            public ItemWithUniques UqComplex { get; set; }
            public Item NonUqComplex { get; set; }
        }

        private class WithEnumerableOfComplex
        {
            public IEnumerable<Item> Items { get; set; }
        }

        private class Item
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }

        private class ItemWithUniques
        {
            [Unique(UniqueModes.PerInstance)]
            public int IntValue { get; set; }
            [Unique(UniqueModes.PerType)]
            public string StringValue { get; set; }
        }
    }
}