using System;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IndexAccessorGetValuesTests : UnitTestBase
    {
        [Test]
        public void GetValues_FromAssignedString_ReturnsAssignedString()
        {
            const string initialValue = "Hello tester!";
            var item = new Dummy { StringProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("StringProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);
            
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValues_FromNullString_ReturnsNullString()
        {
            const string initialValue = null;
            var item = new Dummy { StringProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("StringProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            Assert.IsNull(retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedInt_ReturnsAssignedInt()
        {
            const int initialValue = 12345;
            var item = new Dummy { IntProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("IntProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedDecimal_ReturnsAssignedDecimal()
        {
            const decimal initialValue = 12.56M;
            var item = new Dummy { DecimalProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("DecimalProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedNullableDecimal_ReturnsAssignedNullableDecimal()
        {
            decimal? initialValue = 13.34M;
            var item = new Dummy { NullableDecimalProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableDecimalProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue.GetValueOrDefault() }, retrievedValues);
        }

        [Test]
        public void GetValues_FromNullableDecimalWithNullValue_ReturnsNull()
        {
            decimal? initialValue = null;
            var item = new Dummy { NullableDecimalProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("NullableDecimalProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            Assert.IsNull(retrievedValues);
        }

        [Test]
        public void GetValue_FromAssignedDateTime_ReturnsAssignedDateTime()
        {
            var initialValue = new DateTime(2010, 2, 3);
            var item = new Dummy { DateTimeProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("DateTimeProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValue_FromAssignedBool_ReturnsAssignedBool()
        {
            const bool initialValue = true;
            var item = new Dummy { BoolProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("BoolProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedUInt_ReturnsAssignedInt()
        {
            const uint initialValue = 12345;
            var item = new Dummy { UIntProp = initialValue };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Dummy>("UIntProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        private class Dummy
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public uint UIntProp { get; set; }
            public decimal DecimalProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public bool BoolProp { get; set; }
            public decimal? NullableDecimalProp { get; set; }
        }
    }
}