using System.Linq;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterContainedStructureTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void GetContaineStructureProperties_When_contained_structures_exists_It_should_return_the_properties_holding_the_structure()
        {
            var properties = ReflecterFor().GetContainedStructureProperties(typeof(WithContainedStructures));

            Assert.AreEqual(1, properties.Count(p => p.Path == "Contained1"));
            Assert.AreEqual(1, properties.Count(p => p.Path == "Contained2"));
        }

        [Test]
        public void GetContaineStructureProperties_When_no_contained_structures_exists_It_should_return_empty_array()
        {
            var properties = ReflecterFor().GetContainedStructureProperties(typeof(Structure));

            Assert.AreEqual(0, properties.Length);
        }

        [Test]
        public void GetIndexableProperties_When_contained_structure_Contained_members_are_not_extracted()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithContainedStructure), false);

            Assert.IsFalse(properties.Any());
        }

        [Test]
        public void GetIndexableProperties_When_contained_structure_and_contained_structures_are_allowed_Contained_members_are_extracted()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithContainedStructure), true);

            Assert.AreEqual(2, properties.Count());
            Assert.IsNotNull(properties.SingleOrDefault(p => p.Path == "Contained.StructureId"));
            Assert.IsNotNull(properties.SingleOrDefault(p => p.Path == "Contained.NestedValue"));
        }

        private class WithContainedStructure
        {
            public Structure Contained { get; set; }
        }

        private class WithContainedStructures
        {
            public Structure Contained1 { get; set; }
            public Structure Contained2 { get; set; }
        }

        private class Structure
        {
            public int StructureId { get; set; }
            public int NestedValue { get; set; }
        }
    }
}