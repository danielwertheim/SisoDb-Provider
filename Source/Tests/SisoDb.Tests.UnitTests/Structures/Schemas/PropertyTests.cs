using System;
using System.Collections.Generic;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class PropertyTests : UnitTestBase
    {
        [Test]
        public void IsUnique_WhenRootPropertyWithUniqueInt_ReturnsTrue()
        {
            var property = GetProperty<Dummy>("Uq1");

            Assert.IsTrue(property.IsUnique);
        }

        [Test]
        public void IsEnumerable_WhenIEnumerableOfT_ReturnsTrue()
        {
            var property = GetProperty<DummyWithEnumerable>("Ints");

            Assert.IsTrue(property.IsEnumerable);
        }

        [Test]
        public void IsEnumerable_WhenSimpleType_ReturnsFalse()
        {
            var property = GetProperty<Dummy>("Int1");

            Assert.IsFalse(property.IsEnumerable);
        }

        [Test]
        public void GetValues_WhenIntOnFirstLevel_ReturnsInt()
        {
            var propertyInfo = typeof(Dummy).GetProperty("Int1");
            var property = new StructureProperty(propertyInfo);

            const int expected = 33;
            var item = new Dummy { Int1 = expected };
            var actual = property.GetValues(item);
            
            Assert.AreEqual(new []{expected}, actual);
        }

        private static StructureProperty GetProperty<T>(string name)
        {
            return PropertyTestHelper.GetProperty<T>(name);
        }

        private class Dummy
        {
            public byte Byte1 { get; set; }

            public int Int1 { get; set; }

            public bool Bool1 { get; set; }

            public decimal Decimal1 { get; set; }

            public float Float1 { get; set; }

            public float Double1 { get; set; }

            public DateTime DateTime1 { get; set; }

            public string String1 { get; set; }

            public Guid Guid1 { get; set; }

            public Guid? NullableGuid1 { get; set; }

            public SomeValues Enum1 { get; set; }

            [Unique(UniqueModes.PerInstance)]
            public int Uq1 { get; set; }

            public Address BillingAddress { get; set; }

            public Address DeliveryAddress { get; set; }
        }

        private class Address
        {
            public string Zip { get; set; }
        }

        private enum SomeValues
        {
            A,
            B
        }

        private class DummyWithEnumerable
        {
            public IEnumerable<int> Ints { get; set; }
        }
    }
}