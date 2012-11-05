using System.Collections.Generic;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas.StructurePropertyTests
{
    [TestFixture]
    public class StructurePropertyTests : UnitTestBase
    {
        [Test]
        public void IsUnique_WhenPrimitiveMarkedAsUnique_ReturnsTrue()
        {
            var property = GetProperty<DummyForUniquesTests>("Uq1");

            Assert.IsTrue(property.IsUnique);
            Assert.AreEqual(UniqueModes.PerInstance, property.UniqueMode);
        }

        [Test]
        public void IsUnique_WhenPrimitiveMarkedAsNonUnique_ReturnsFalse()
        {
            var property = GetProperty<DummyForUniquesTests>("Int1");

            Assert.IsFalse(property.IsUnique);
        }

        [Test]
        public void IsEnumerable_WhenIEnumerableOfSimpleType_ReturnsTrue()
        {
            var property = GetProperty<DummyForEnumerableTests>("Ints");

            Assert.IsTrue(property.IsEnumerable);
        }

        [Test]
        public void IsEnumerable_WhenPrimitiveType_ReturnsFalse()
        {
            var property = GetProperty<DummyForEnumerableTests>("Int1");

            Assert.IsFalse(property.IsEnumerable);
        }

        private static IStructureProperty GetProperty<T>(string name) where T : class
        {
            return StructurePropertyTestFactory.GetPropertyByPath<T>(name);
        }

        private class DummyForUniquesTests
        {
            public int Int1 { get; set; }

            [Unique(UniqueModes.PerInstance)]
            public int Uq1 { get; set; }
        }

        private class DummyForEnumerableTests
        {
            public int Int1 { get; set; }

            public IEnumerable<int> Ints { get; set; }
        }
    }
}