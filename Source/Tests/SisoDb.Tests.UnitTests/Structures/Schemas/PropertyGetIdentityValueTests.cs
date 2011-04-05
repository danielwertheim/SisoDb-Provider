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
            var intPropertyInfo = typeof(IdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            const int expected = 42;
            var item = new IdentityOnRoot { SisoId = expected };

            var actual = intProperty.GetIdValue<IdentityOnRoot, int>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(NullableIdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            const int expectedInt = 42;
            var item = new NullableIdentityOnRoot { SisoId = expectedInt };

            var actual = intProperty.GetIdValue<NullableIdentityOnRoot, int>(item);

            Assert.AreEqual(expectedInt, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(NullableIdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            var item = new NullableIdentityOnRoot { SisoId = null };

            var actual = intProperty.GetIdValue<NullableIdentityOnRoot, int>(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetIdValue_WhenIntNotOnFirstLevel_ThrowsSisoDbException()
        {
            var itemPropertyInfo = typeof(Container).GetProperty("IdentityOnRootItem");
            var itemProperty = new StructureProperty(itemPropertyInfo);

            var intPropertyInfo = typeof(IdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(itemProperty, intPropertyInfo);

            var item = new Container { IdentityOnRootItem = new IdentityOnRoot { SisoId = 42 } };

            var ex = Assert.Throws<SisoDbException>(() => intProperty.GetIdValue<Container, int>(item));

            Assert.AreEqual(ExceptionMessages.Property_GetIdValue_InvalidLevel, ex.Message);
        }

        private class IdentityOnRoot
        {
            public int SisoId { get; set; }
        }

        private class NullableIdentityOnRoot
        {
            public int? SisoId { get; set; }
        }

        private class Container
        {
            public IdentityOnRoot IdentityOnRootItem { get; set; }
        }
    }
}