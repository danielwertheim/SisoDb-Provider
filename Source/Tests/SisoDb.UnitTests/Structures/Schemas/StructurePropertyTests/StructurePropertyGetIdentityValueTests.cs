using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructurePropertyTests
{
    [TestFixture]
    public class StructurePropertyGetIdentityValueTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenIntOnFirstLevel_ReturnsInt()
        {
            const int expected = 42;
            var item = new IdentityOnRoot { StructureId = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<IdentityOnRoot>("StructureId");
            
            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableIntOnFirstLevel_ReturnsInt()
        {
            const int expectedInt = 42;
            var item = new NullableIdentityOnRoot { StructureId = expectedInt };
            var property = StructurePropertyTestFactory.GetPropertyByPath<NullableIdentityOnRoot>("StructureId");

            var actual = property.GetValue(item);

            Assert.AreEqual(expectedInt, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableIntOnFirstLevel_ReturnsInt()
        {
            var item = new NullableIdentityOnRoot { StructureId = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<NullableIdentityOnRoot>("StructureId");
            
            var actual = property.GetValue(item);

            Assert.IsNull(actual);
        }

        private class IdentityOnRoot
        {
            public int StructureId { get; set; }
        }

        private class NullableIdentityOnRoot
        {
            public int? StructureId { get; set; }
        }
    }
}