using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterComplexIndexablePropertiesTests : UnitTestBase
    {
        private readonly IStructureTypeReflecter _reflecter = new StructureTypeReflecter();

        [Test]
        public void GetComplexIndexableProperties_WhenRootWithSimpleAndComplexProperties_ReturnsOnlyComplexProperties()
        {
            var properties = _reflecter.GetComplexIndexablePropertyInfos(typeof(WithSimpleAndComplexProperties));

            Assert.AreEqual(1, properties.Count());
        }

        [Test]
        public void GetComplexProperties_WhenRootWithUniqeAndNonUniqueComplexProperties_ReturnsComplexUniqueProperties()
        {
            var properties = _reflecter.GetComplexIndexablePropertyInfos(typeof(WithUniqueAndNonUniqueComplexProperties));

            Assert.AreEqual(2, properties.Count());
        }

        [Test]
        public void GetComplexProperties_WhenRootWithEnumerable_EnumerableMemberIsNotReturnedAsComplex()
        {
            var properties = _reflecter.GetComplexIndexablePropertyInfos(typeof(WithEnumerable));

            Assert.AreEqual(0, properties.Count());
        }

        [Test]
        public void GetComplexProperties_WhenItIsContainedStructure_NotExtracted()
        {
            var properties = _reflecter.GetComplexIndexablePropertyInfos(typeof(WithContainedStructure));

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

        private class WithContainedStructure
        {
            public int Id { get; set; }

            public Structure ContainedStructure { get; set; }
        }

        private class Structure
        {
            public int Id { get; set; }
        }
    }
}