using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.TypeInfoTests
{
    [TestFixture]
    public class TypeInfoSimpleIndexablePropertiesTests : UnitTestBase
    {
        [Test]
        public void GetSimpleIndexableProperties_WhenMultiplePublicSimplePropertiesExistsAndNoExclusions_ReturnsAllPublicSimpleProperties()
        {
            var properties = TypeInfo<WithSimpleProperties>.GetSimpleIndexablePropertyInfos();

            var names = properties.Select(p => p.Name).ToArray();
            Assert.AreEqual(6, properties.Count());
            CollectionAssert.Contains(names, "Id");
            CollectionAssert.Contains(names, "Age");
            CollectionAssert.Contains(names, "Name");
            CollectionAssert.Contains(names, "DateOfBirth");
            CollectionAssert.Contains(names, "Wage");
            CollectionAssert.Contains(names, "Byte");
        }

        [Test]
        public void GetSimpleIndexableProperties_WhenByteArray_NotReturned()
        {
            var properties = TypeInfo<WithNonSimpleProperties>.GetSimpleIndexablePropertyInfos();

            Assert.AreEqual(0, properties.Count());
        }

        [Test]
        public void GetSimpleIndexableProperties_WhenExclusionIsPassed_DoesNotReturnExcludedProperties()
        {
            var properties = TypeInfo<WithSimpleProperties>.GetSimpleIndexablePropertyInfos(new[] { "Id", "Name" });

            var names = properties.Select(p => p.Name).ToArray();
            CollectionAssert.DoesNotContain(names, "Id");
            CollectionAssert.DoesNotContain(names, "Name");
        }

        [Test]
        public void GetSimpleIndexableProperties_WhenExclusionIsPassed_DoesReturnNonExcludedProperties()
        {
            var properties = TypeInfo<WithSimpleProperties>.GetSimpleIndexablePropertyInfos(new[] { "Id", "Name" });

            var names = properties.Select(p => p.Name).ToArray();
            CollectionAssert.Contains(names, "Age");
            CollectionAssert.Contains(names, "DateOfBirth");
            CollectionAssert.Contains(names, "Wage");
        }

        [Test]
        public void GetSimpleIndexableProperties_WhenSimplePrivatePropertyExists_PrivatePropertyIsNotReturned()
        {
            var properties = TypeInfo<WithPrivateProperty>.GetSimpleIndexablePropertyInfos();

            Assert.AreEqual(0, properties.Count());
        }

        [Test]
        public void GetSimpleIndexableProperties_WhenSimpleAndComplexPropertiesExists_ReturnsOnlySimpleProperties()
        {
            var properties = TypeInfo<WithSimpleAndComplexProperties>.GetSimpleIndexablePropertyInfos();

            var complex = properties.Where(p => !p.PropertyType.IsSimpleType());
            var names = properties.Select(p => p.Name).ToArray();
            Assert.AreEqual(0, complex.Count());
            Assert.AreEqual(2, properties.Count());
            CollectionAssert.Contains(names, "SimpleIntProperty");
            CollectionAssert.Contains(names, "SimpleStringProperty");
        }

        [Test]
        public void GetSimpleIndexableProperties_WhenUniquesExists_ReturnsSimpleUniqueProperties()
        {
            var properties = TypeInfo<WithUniqueIndexes>.GetSimpleIndexablePropertyInfos();

            var names = properties.Select(p => p.Name).ToArray();
            Assert.AreEqual(2, properties.Count());
            CollectionAssert.Contains(names, "UqInt");
            CollectionAssert.Contains(names, "UqString");
        }

        private class WithSimpleProperties
        {
            public Guid Id { get; set; }

            public int Age { get; set; }

            public string Name { get; set; }

            public DateTime DateOfBirth { get; set; }

            [Unique(UniqueModes.PerInstance)]
            public decimal Wage { get; set; }

            public byte Byte { get; set; }
        }

        private class WithPrivateProperty
        {
            private int Int { get; set; }
        }

        private class WithNonSimpleProperties
        {
            public byte[] Bytes { get; set; }
        }

        private class WithSimpleAndComplexProperties
        {
            public string SimpleStringProperty { get; set; }

            public int SimpleIntProperty { get; set; }

            public WithSimpleProperties ComplexProperty { get; set; }
        }

        private class WithUniqueIndexes
        {
            [Unique(UniqueModes.PerInstance)]
            public int UqInt { get; set; }

            [Unique(UniqueModes.PerInstance)]
            public string UqString { get; set; }

            [Unique(UniqueModes.PerInstance)]
            public WithSimpleProperties UqComplex1 { get; set; }

            public WithSimpleProperties UqComplex2 { get; set; }
        }
    }
}