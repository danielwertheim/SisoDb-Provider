using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterUniquesPropertiesTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void GetIndexableProperties_WhenSimpleUniquesExistsOnRoot_ReturnsSimpleUniques()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithSimpleUniques));

            Assert.AreEqual(2, properties.Count());

            CustomAssert.Exists(properties, p => p.Name == "UqIntOnLevel1");
            CustomAssert.Exists(properties, p => p.Name == "UqStringOnLevel1");
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithExplicitUniqueOnChildWithNoUniques_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => ReflecterFor().GetIndexableProperties(typeof(WithExplicitUniqueOnChildWithoutUniques)));
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithExplicitUniqueOnChildWithUniques_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => ReflecterFor().GetIndexableProperties(typeof(WithExplicitUniqueOnChildWithUniques)));
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithImplicitUniqueOnChildWithUniques_ChildUniquesAreExtracted()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithImplicitUniqueOnChildWithUniques));

            var uniques = properties.Where(p => p.IsUnique).ToList();
            Assert.AreEqual(1, uniques.Count);
            Assert.AreEqual("Child.Code", uniques[0].Path);
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithUniqueEnumerableOfSimple_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => ReflecterFor().GetIndexableProperties(typeof(WithUniqueEnumerableOfSimple)));
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithUniqueEnumerableOfComplexWithUnique_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => ReflecterFor().GetIndexableProperties(typeof(WithUniqueEnumerableOfComplexWithUnique)));
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithEnumerableOfComplexWithUnique_UniqueIsExtracted()
        {
            var properties = ReflecterFor().GetIndexableProperties(typeof(WithEnumerableOfComplexWithUnique));

            var uniques = properties.Where(p => p.IsUnique).ToList();
            Assert.AreEqual(1, uniques.Count);
            Assert.AreEqual("Items.Code", uniques[0].Path);
        }

        private class WithSimpleUniques
        {
            [Unique(UniqueModes.PerType)]
            public int UqIntOnLevel1 { get; set; }

            [Unique(UniqueModes.PerInstance)]
            public string UqStringOnLevel1 { get; set; }
        }

        private class WithExplicitUniqueOnChildWithoutUniques
        {
            [Unique(UniqueModes.PerInstance)]
            public ChildWithoutUnique Child { get; set; }
        }

        private class WithExplicitUniqueOnChildWithUniques
        {
            [Unique(UniqueModes.PerInstance)]
            public ChildWithUnique Child { get; set; }
        }

        private class WithImplicitUniqueOnChildWithUniques
        {
            public ChildWithUnique Child { get; set; }
        }

        private class WithUniqueEnumerableOfSimple
        {
            [Unique(UniqueModes.PerInstance)]
            public IEnumerable<string> Items { get; set; }
        }

        private class WithUniqueEnumerableOfComplexWithUnique
        {
            [Unique(UniqueModes.PerInstance)]
            public IEnumerable<ChildWithUnique> Items { get; set; }
        }

        private class WithEnumerableOfComplexWithUnique
        {
            public IEnumerable<ChildWithUnique> Items { get; set; }
        }

        private class ChildWithoutUnique
        {
            public string Name { get; set; }
        }

        private class ChildWithUnique
        {
            [Unique(UniqueModes.PerInstance)]
            public int Code { get; set; }
        }
    }
}