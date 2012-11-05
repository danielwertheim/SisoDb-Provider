using System;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructurePropertyTests
{
    [TestFixture]
    public class StructurePropertyGetGuidValueTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenGuidOnFirstLevel_ReturnsGuid()
        {
            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new GuidOnRoot { StructureId = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<GuidOnRoot>("StructureId");
            
            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableGuidOnFirstLevel_ReturnsGuid()
        {
            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new NullableGuidOnRoot { StructureId = expected };
            var property = StructurePropertyTestFactory.GetPropertyByPath<NullableGuidOnRoot>("StructureId");
            
            var actual = property.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableGuidOnFirstLevel_ReturnsNull()
        {
            var item = new NullableGuidOnRoot { StructureId = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<NullableGuidOnRoot>("StructureId");
            
            var actual = property.GetValue(item);

            Assert.IsNull(actual);
        }

        

        private class GuidOnRoot
        {
            public Guid StructureId { get; set; }
        }

        private class NullableGuidOnRoot
        {
            public Guid? StructureId { get; set; }
        }

        private class Container
        {
            public GuidOnRoot GuidOnRootItem { get; set; }
        }
    }
}