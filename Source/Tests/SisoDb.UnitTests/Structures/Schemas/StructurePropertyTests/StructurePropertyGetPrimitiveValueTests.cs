using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructurePropertyTests
{
    [TestFixture]
    public class StructurePropertyGetPrimitiveValueTests : UnitTestBase
    {
        [Test]
        public void GetValue_WhenAssignedInt_ReturnsAssignedValue()
        {
            const int expected = 33;
            var item = new Dummy { Int1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("Int1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenAssignedNullableInt_ReturnsAssignedValue()
        {
            const int expected = 33;
            var item = new Dummy { NullableInt1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableInt1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenUnAssignedNullableInt_ReturnsNull()
        {
            var item = new Dummy { NullableInt1 = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableInt1");

            var actual = property.GetValue(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetValue_WhenAssignedBool_ReturnsAssignedValue()
        {
            const bool expected = true;
            var item = new Dummy { Bool1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("Bool1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenAssignedNullableBool_ReturnsAssignedValue()
        {
            const bool expected = true;
            var item = new Dummy { NullableBool1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableBool1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenUnAssignedNullableBool_ReturnsNull()
        {
            var item = new Dummy { NullableBool1 = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableBool1");

            var actual = property.GetValue(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetValue_WhenAssignedDecimal_ReturnsAssignedValue()
        {
            const decimal expected = 1.33m;
            var item = new Dummy { Decimal1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("Decimal1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenAssignedNullableDecimal_ReturnsAssignedValue()
        {
            const decimal expected = 1.33m;
            var item = new Dummy { NullableDecimal1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableDecimal1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenUnAssignedNullableDecimal_ReturnsNull()
        {
            var item = new Dummy { NullableDecimal1 = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableDecimal1");

            var actual = property.GetValue(item);
            
            Assert.IsNull(actual);
        }

        [Test]
        public void GetValue_WhenAssignedUInt_ReturnsAssignedValue()
        {
            const uint expected = 33;
            var item = new Dummy { UInt1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("UInt1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenAssignedNullableUInt_ReturnsAssignedValue()
        {
            const uint expected = 33;
            var item = new Dummy { NullableUInt1 = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableUInt1");

            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetValue_WhenUnAssignedNullableUInt_ReturnsNull()
        {
            var item = new Dummy { NullableUInt1 = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableUInt1");

            var actual = property.GetValue(item);

            Assert.IsNull(actual);
        }

        private class Dummy
        {
            public int Int1 { get; set; }
            public uint UInt1 { get; set; }
            public bool Bool1 { get; set; }
            public decimal Decimal1 { get; set; }
            public int? NullableInt1 { get; set; }
            public uint? NullableUInt1 { get; set; }
            public bool? NullableBool1 { get; set; }
            public decimal? NullableDecimal1 { get; set; }
        }
    }
}