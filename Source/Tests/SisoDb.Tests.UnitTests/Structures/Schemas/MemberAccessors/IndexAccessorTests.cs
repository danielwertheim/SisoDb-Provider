using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IndexAccessorTests : UnitTestBase
    {
        [Test]
        public void GetValues_FromAssignedString_ReturnsAssignedString()
        {
            const string initialValue = "Hello tester!";
            var item = new Dummy { StringProp = initialValue };

            var property = GetProperty("StringProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValues_FromNullString_ReturnsNullString()
        {
            const string initialValue = null;
            var item = new Dummy { StringProp = initialValue };

            var property = GetProperty("StringProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            Assert.IsNull(retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedInt_ReturnsAssignedInt()
        {
            const int initialValue = 12345;
            var item = new Dummy { IntProp = initialValue };

            var property = GetProperty("IntProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedDecimal_ReturnsAssignedDecimal()
        {
            const decimal initialValue = 12.56M;
            var item = new Dummy { DecimalProp = initialValue };

            var property = GetProperty("DecimalProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(
                new[] { initialValue },
                retrievedValues);
        }

        [Test]
        public void GetValues_FromAssignedNullableDecimal_ReturnsAssignedNullableDecimal()
        {
            decimal? initialValue = 13.34M;
            var item = new Dummy { NullableDecimalProp = initialValue };

            var property = GetProperty("NullableDecimalProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(
                new[] { initialValue.GetValueOrDefault() },
                retrievedValues);
        }

        [Test]
        public void GetValues_FromNullableDecimalWithNullValue_ReturnsNull()
        {
            decimal? initialValue = null;
            var item = new Dummy { NullableDecimalProp = initialValue };

            var property = GetProperty("NullableDecimalProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            Assert.IsNull(retrievedValues);
        }

        [Test]
        public void GetValue_FromAssignedDateTime_ReturnsAssignedDateTime()
        {
            var initialValue = new DateTime(2010, 2, 3);
            var item = new Dummy { DateTimeProp = initialValue };

            var property = GetProperty("DateTimeProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(
                new[] { initialValue },
                retrievedValues);
        }

        [Test]
        public void GetValue_FromAssignedBool_ReturnsAssignedBool()
        {
            const bool initialValue = true;
            var item = new Dummy { BoolProp = initialValue };

            var property = GetProperty("BoolProp");
            var indexAccessor = new IndexAccessor(property);
            var retrievedValues = indexAccessor.GetValues(item);

            CollectionAssert.AreEqual(new[] { initialValue }, retrievedValues);
        }

        private static IProperty GetProperty(string name)
        {
            return StructureType<Dummy>.Instance.IndexableProperties.Where(p => p.Name == name).Single();
        }

        private class Dummy
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public decimal DecimalProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public bool BoolProp { get; set; }
            public decimal? NullableDecimalProp { get; set; }
        }
    }
}