using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterGetIndexablePropertiesExceptTests : UnitTestBase
    {
        [Test]
        public void GetIndexablePropertiesExcept_WhenCalledWithNullType_ThrowsArgumentNullException()
        {
            var reflecter = new StructureTypeReflecter();

            var ex = Assert.Throws<ArgumentNullException>(() => reflecter.GetIndexablePropertiesExcept(null, null));

            Assert.AreEqual("type", ex.ParamName);
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenCalledWithNullExlcudes_ThrowsArgumentNullException()
        {
            var reflecter = new StructureTypeReflecter();

            var ex = Assert.Throws<ArgumentNullException>(() => reflecter.GetIndexablePropertiesExcept(typeof(WithSisoId), null));

            Assert.AreEqual("nonIndexablePaths", ex.ParamName);
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenCalledWithNoExlcudes_ThrowsArgumentNullException()
        {
            var reflecter = new StructureTypeReflecter();

            var ex = Assert.Throws<ArgumentNullException>(() => reflecter.GetIndexablePropertiesExcept(typeof(WithSisoId), new string[] { }));

            Assert.AreEqual("nonIndexablePaths", ex.ParamName);
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingSisoId_PropertyIsNotReturned()
        {
            var reflecter = new StructureTypeReflecter();
            
            var properties = reflecter.GetIndexablePropertiesExcept(typeof (WithSisoId), new[] {"SisoId"});

            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "SisoId"));
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenBytesArrayExists_PropertyIsNotReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetIndexablePropertiesExcept(typeof(WithSisoId), new[] { "" });

            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "Bytes1"));
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingAllProperties_NoPropertiesAreReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetIndexablePropertiesExcept(typeof(WithSisoId), new[] { "Bool1", "DateTime1", "String1", "Nested", "Nested.Int1OnNested", "Nested.String1OnNested" });

            Assert.AreEqual(0, properties.Count());
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingComplexNested_NoNestedPropertiesAreReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetIndexablePropertiesExcept(typeof(WithSisoId), new[] { "Nested" });

            Assert.AreEqual(0, properties.Count(p => p.Path.StartsWith("Nested")));
        }

        [Test]
        public void GetIndexablePropertiesExcept_WhenExcludingNestedSimple_OtherSimpleNestedPropertiesAreReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetIndexablePropertiesExcept(typeof(WithSisoId), new[] { "Nested.String1OnNested" });

            Assert.AreEqual(1, properties.Count(p => p.Path.StartsWith("Nested")));
            Assert.AreEqual(1, properties.Count(p => p.Path == "Nested.Int1OnNested"));
        }

        private class WithSisoId
        {
            public int SisoId { get; set; }

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