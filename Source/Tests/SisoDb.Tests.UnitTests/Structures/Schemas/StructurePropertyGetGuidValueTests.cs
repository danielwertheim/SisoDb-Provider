using System;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructurePropertyGetGuidValueTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenGuidOnFirstLevel_ReturnsGuid()
        {
            var guidPropertyInfo = typeof(GuidOnRoot).GetProperty("SisoId");
            var guidProperty = new StructureProperty(guidPropertyInfo);

            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new GuidOnRoot { SisoId = expected };

            var actual = guidProperty.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableGuidOnFirstLevel_ReturnsGuid()
        {
            var intPropertyInfo = typeof(NullableGuidOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new NullableGuidOnRoot { SisoId = expected };

            var actual = intProperty.GetValue(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableGuidOnFirstLevel_ReturnsNull()
        {
            var intPropertyInfo = typeof(NullableGuidOnRoot).GetProperty("SisoId");
            var intProperty = new StructureProperty(intPropertyInfo);

            var item = new NullableGuidOnRoot { SisoId = null };

            var actual = intProperty.GetValue(item);

            Assert.IsNull(actual);
        }

        

        private class GuidOnRoot
        {
            public Guid SisoId { get; set; }
        }

        private class NullableGuidOnRoot
        {
            public Guid? SisoId { get; set; }
        }

        private class Container
        {
            public GuidOnRoot GuidOnRootItem { get; set; }
        }
    }
}