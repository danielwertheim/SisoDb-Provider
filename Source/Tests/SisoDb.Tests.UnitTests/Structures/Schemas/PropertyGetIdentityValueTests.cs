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
            var intPropertyInfo = typeof(IdentityOnRoot).GetProperty("Id");
            var intProperty = new Property(intPropertyInfo);

            const int expected = 42;
            var item = new IdentityOnRoot { Id = expected };

            var actual = intProperty.GetIdValue<IdentityOnRoot, int>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(NullableIdentityOnRoot).GetProperty("Id");
            var intProperty = new Property(intPropertyInfo);

            const int expectedInt = 42;
            var item = new NullableIdentityOnRoot { Id = expectedInt };

            var actual = intProperty.GetIdValue<NullableIdentityOnRoot, int>(item);

            Assert.AreEqual(expectedInt, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(NullableIdentityOnRoot).GetProperty("Id");
            var intProperty = new Property(intPropertyInfo);

            var item = new NullableIdentityOnRoot { Id = null };

            var actual = intProperty.GetIdValue<NullableIdentityOnRoot, int>(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetIdValue_WhenIntNotOnFirstLevel_ThrowsSisoDbException()
        {
            var itemPropertyInfo = typeof(Container).GetProperty("IdentityOnRootItem");
            var itemProperty = new Property(itemPropertyInfo);

            var intPropertyInfo = typeof(IdentityOnRoot).GetProperty("Id");
            var intProperty = new Property(1, itemProperty, intPropertyInfo);

            var item = new Container { IdentityOnRootItem = new IdentityOnRoot { Id = 42 } };

            var ex = Assert.Throws<SisoDbException>(() => intProperty.GetIdValue<Container, int>(item));

            Assert.AreEqual(ExceptionMessages.Property_GetIdValue_InvalidLevel, ex.Message);
        }

        private class IdentityOnRoot
        {
            public int Id { get; set; }
        }

        private class NullableIdentityOnRoot
        {
            public int? Id { get; set; }
        }

        private class Container
        {
            public IdentityOnRoot IdentityOnRootItem { get; set; }
        }
    }
}