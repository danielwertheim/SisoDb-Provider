using System;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterGetIndexablePropertiesExceptTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void GetIndexablePropertiesExcept_WhenCalledWithNullExlcudes_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, null));

            Assert.AreEqual("nonIndexablePaths", ex.ParamName);
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenCalledWithNoExlcudes_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentException>(() => ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, new string[] { }));

            Assert.AreEqual("nonIndexablePaths", ex.ParamName);
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingStructureId_PropertyIsNotReturned()
        {
            var properties = ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, new[] { "StructureId" });

            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "StructureId"));
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenBytesArrayExists_PropertyIsNotReturned()
        {
            var properties = ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, new[] { "" });

            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "Bytes1"));
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingAllProperties_NoPropertiesAreReturned()
        {
            var properties = ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, new[] { "StructureId", "Bool1", "DateTime1", "String1", "Nested", "Nested.Int1OnNested", "Nested.String1OnNested" });

            Assert.AreEqual(0, properties.Count());
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingComplexNested_NoNestedPropertiesAreReturned()
        {
            var properties = ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, new[] { "Nested" });

            Assert.AreEqual(0, properties.Count(p => p.Path.StartsWith("Nested")));
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingNestedSimple_OtherSimpleNestedPropertiesAreReturned()
        {
            var properties = ReflecterFor().GetIndexablePropertiesExcept(typeof(WithStructureId), false, new[] { "Nested.String1OnNested" });

            Assert.AreEqual(1, properties.Count(p => p.Path.StartsWith("Nested")));
            Assert.AreEqual(1, properties.Count(p => p.Path == "Nested.Int1OnNested"));
        }

        private class WithStructureId
        {
            public int StructureId { get; set; }

            public bool Bool1 { get; set; }

            public DateTime DateTime1 { get; set; }

            public string String1 { get; set; }

            public byte[] Bytes1 { get; set; }

            public Nested Nested { get; set; }
        }

        private class Nested
        {
            public int Int1OnNested { get; set; }

            public string String1OnNested { get; set; }
        }
    }
}