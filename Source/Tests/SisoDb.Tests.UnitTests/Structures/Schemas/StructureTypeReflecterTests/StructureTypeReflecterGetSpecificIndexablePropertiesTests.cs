using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterGetSpecificIndexablePropertiesTests : UnitTestBase
    {
        [Test]
        public void GetSpecificIndexableProperties_WhenCalledWithNullType_ThrowsArgumentNullException()
        {
            var reflecter = new StructureTypeReflecter();

            var ex = Assert.Throws<ArgumentNullException>(() => reflecter.GetSpecificIndexableProperties(null, null));

            Assert.AreEqual("type", ex.ParamName);
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenCalledWithNullExlcudes_ThrowsArgumentNullException()
        {
            var reflecter = new StructureTypeReflecter();

            var ex = Assert.Throws<ArgumentNullException>(() => reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), null));

            Assert.AreEqual("indexablePaths", ex.ParamName);
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenCalledWithNoExlcudes_ThrowsArgumentNullException()
        {
            var reflecter = new StructureTypeReflecter();

            var ex = Assert.Throws<ArgumentNullException>(() => reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), new string[] { }));

            Assert.AreEqual("indexablePaths", ex.ParamName);
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenIncludingSisoId_PropertyIsNotReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), new[] { "SisoId" });

            Assert.AreEqual(0, properties.Count());
            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "SisoId"));
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenIncludingNonSisoId_SisoIdPropIsNotReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), new[] { "Int1" });

            Assert.AreEqual(1, properties.Count());
            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "SisoId"));
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenIncludingBytesArray_PropertyIsNotReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), new[] { "Bytes1" });

            Assert.AreEqual(0, properties.Count());
            Assert.IsNull(properties.SingleOrDefault(p => p.Path == "Bytes1"));
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenIncludingProperty_PropertyIsReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), new[] { "Bool1" });

            Assert.AreEqual(1, properties.Count());
            Assert.IsNotNull(properties.SingleOrDefault(p => p.Path == "Bool1"));
        }

        [Test]
        public void GetSpecificIndexableProperties_WhenIncludingNestedProperty_PropertyIsReturned()
        {
            var reflecter = new StructureTypeReflecter();

            var properties = reflecter.GetSpecificIndexableProperties(typeof(WithSisoId), new[] { "Nested", "Nested.Int1OnNested" });

            Assert.AreEqual(1, properties.Count());
            Assert.IsNotNull(properties.SingleOrDefault(p => p.Path == "Nested.Int1OnNested"));
        }

        private class WithSisoId
        {
            public int SisoId { get; set; }

            public bool Int1 { get; set; }

            public bool Bool1 { get; set; }

            public DateTime DateTime1 { get; set; }

            public string String1 { get; set; }

            public byte[] Bytes1 { get; set; }

            public Nested Nested { get; set; }
        }

        private class Nested
        {
            public int Int1OnNested { get; set; }
        }
    }
}