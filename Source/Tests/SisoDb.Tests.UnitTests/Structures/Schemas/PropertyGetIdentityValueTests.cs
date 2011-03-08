using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class PropertyGetIdentityValueTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(Dummy2).GetProperty("Id");
            var intProperty = new Property(intPropertyInfo);

            const int expected = 42;
            var item = new Dummy2 { Id = expected };

            var actual = intProperty.GetIdValue<Dummy2, int>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(Dummy2).GetProperty("NullableId");
            var intProperty = new Property(intPropertyInfo);

            const int expectedInt = 42;
            var item = new Dummy2 { NullableId = expectedInt };

            var actual = intProperty.GetIdValue<Dummy2, int>(item);

            Assert.AreEqual(expectedInt, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(Dummy2).GetProperty("NullableId");
            var intProperty = new Property(intPropertyInfo);

            var item = new Dummy2 { NullableId = null };

            var actual = intProperty.GetIdValue<Dummy2, int>(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetIdValue_WhenIntNotOnFirstLevel_ThrowsSisoDbException()
        {
            var dummy2PropertyInfo = typeof(Dummy1).GetProperty("Item");
            var dummy2Property = new Property(dummy2PropertyInfo);

            var intPropertyInfo = typeof(Dummy2).GetProperty("Id");
            var intProperty = new Property(1, dummy2Property, intPropertyInfo);

            var item = new Dummy1 { Item = new Dummy2 { Id = 42 } };

            var ex = Assert.Throws<SisoDbException>(() => intProperty.GetIdValue<Dummy1, int>(item));

            Assert.AreEqual(ExceptionMessages.Property_GetIdValue_InvalidLevel, ex.Message);
        }

        public class Dummy1
        {
            public Dummy2 Item { get; set; }
        }

        public class Dummy2
        {
            public int Id { get; set; }

            public int? NullableId { get; set; }
        }
    }
}