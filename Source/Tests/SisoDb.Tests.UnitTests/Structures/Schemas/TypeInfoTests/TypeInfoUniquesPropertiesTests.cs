using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.TypeInfoTests
{
    [TestFixture]
    public class TypeInfoUniquesPropertiesTests : UnitTestBase
    {
        [Test]
        public void GetIndexableProperties_WhenSimpleUniquesExistsOnRoot_ReturnsSimpleUniques()
        {
            var properties = TypeInfo<WithSimpleUniques>.GetIndexableProperties();

            CustomAssert.Count(2, properties);
            CustomAssert.Exists(properties, p => p.Name == "UqIntOnLevel1");
            CustomAssert.Exists(properties, p => p.Name == "UqStringOnLevel1");
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithExplicitUniqueOnChildWithNoUniques_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => TypeInfo<WithExplicitUniqueOnChildWithoutUniques>.GetIndexableProperties());
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithExplicitUniqueOnChildWithUniques_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => TypeInfo<WithExplicitUniqueOnChildWithUniques>.GetIndexableProperties());
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithImplicitUniqueOnChildWithUniques_ChildUniquesAreExtracted()
        {
            var properties = TypeInfo<WithImplicitUniqueOnChildWithUniques>.GetIndexableProperties();

            var uniques = properties.Where(p => p.IsUnique).ToList();
            CustomAssert.Count(1, uniques);
            Assert.AreEqual("Child.Code", uniques[0].Path);
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithUniqueEnumerableOfSimple_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => TypeInfo<WithUniqueEnumerableOfSimple>.GetIndexableProperties());
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithUniqueEnumerableOfComplexWithUnique_ThrowsSisoDbException()
        {
            Assert.Throws<SisoDbException>(
                () => TypeInfo<WithUniqueEnumerableOfComplexWithUnique>.GetIndexableProperties());
        }

        [Test]
        public void GetIndexableProperties_WhenRootWithEnumerableOfComplexWithUnique_UniqueIsExtracted()
        {
            var properties = TypeInfo<WithEnumerableOfComplexWithUnique>.GetIndexableProperties();

            var uniques = properties.Where(p => p.IsUnique).ToList();
            CustomAssert.Count(1, uniques);
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