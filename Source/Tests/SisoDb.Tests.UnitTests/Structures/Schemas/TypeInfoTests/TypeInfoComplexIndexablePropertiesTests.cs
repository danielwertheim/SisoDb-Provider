using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.TypeInfoTests
{
    [TestFixture]
    public class TypeInfoComplexIndexablePropertiesTests : UnitTestBase
    {
        [Test]
        public void GetComplexIndexableProperties_WhenRootWithSimpleAndComplexProperties_ReturnsOnlyComplexProperties()
        {
            var properties = TypeInfo<WithSimpleAndComplexProperties>.GetComplexIndexablePropertyInfos();

            Assert.AreEqual(1, properties.Count());
        }

        [Test]
        public void GetComplexProperties_WhenRootWithUniqeAndNonUniqueComplexProperties_ReturnsComplexUniqueProperties()
        {
            var properties = TypeInfo<WithUniqueAndNonUniqueComplexProperties>.GetComplexIndexablePropertyInfos();

            Assert.AreEqual(2, properties.Count());
        }

        [Test]
        public void GetComplexProperties_WhenRootWithEnumerable_EnumerableMemberIsNotReturnedAsComplex()
        {
            var properties = TypeInfo<WithEnumerable>.GetComplexIndexablePropertyInfos();

            Assert.AreEqual(0, properties.Count());
        }

        private class Item
        {}

        private class WithSimpleAndComplexProperties
        {
            public string SimpleStringProperty { get; set; }

            public int SimpleIntProperty { get; set; }

            public Item ComplexProperty { get; set; }
        }

        private class WithUniqueAndNonUniqueComplexProperties
        {
            [Unique(UniqueModes.PerInstance)]
            public Item UqComplex1 { get; set; }

            public Item UqComplex2 { get; set; }
        }

        private class WithEnumerable
        {
            public IEnumerable<Item> Dummies { get; set; }
        }
    }
}