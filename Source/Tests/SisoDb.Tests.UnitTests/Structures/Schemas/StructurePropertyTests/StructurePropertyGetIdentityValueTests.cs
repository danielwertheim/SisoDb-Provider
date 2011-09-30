using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.StructurePropertyTests
{
    [TestFixture]
    public class StructurePropertyGetIdentityValueTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(IdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            const int expected = 42;
            var item = new IdentityOnRoot { SisoId = expected };

            var actual = intProperty.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(NullableIdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            const int expectedInt = 42;
            var item = new NullableIdentityOnRoot { SisoId = expectedInt };

            var actual = intProperty.GetValue(item);

            Assert.AreEqual(expectedInt, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableIntOnFirstLevel_ReturnsInt()
        {
            var intPropertyInfo = typeof(NullableIdentityOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            var item = new NullableIdentityOnRoot { SisoId = null };

            var actual = intProperty.GetValue(item);

            Assert.IsNull(actual);
        }

        private class IdentityOnRoot
        {
            public int SisoId { get; set; }
        }

        private class NullableIdentityOnRoot
        {
            public int? SisoId { get; set; }
        }
    }
}