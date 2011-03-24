using System;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class PropertyGetGuidValueTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenGuidOnFirstLevel_ReturnsGuid()
        {
            var guidPropertyInfo = typeof(GuidOnRoot).GetProperty("SisoId");
            var guidProperty = new Property(guidPropertyInfo);

            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new GuidOnRoot { SisoId = expected };

            var actual = guidProperty.GetIdValue<GuidOnRoot, Guid>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableGuidOnFirstLevel_ReturnsGuid()
        {
            var intPropertyInfo = typeof(NullableGuidOnRoot).GetProperty("SisoId");
            var intProperty = new Property(intPropertyInfo);

            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new NullableGuidOnRoot { SisoId = expected };

            var actual = intProperty.GetIdValue<NullableGuidOnRoot, Guid>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableGuidOnFirstLevel_ReturnsNull()
        {
            var intPropertyInfo = typeof(NullableGuidOnRoot).GetProperty("SisoId");
            var intProperty = new Property(intPropertyInfo);

            var item = new NullableGuidOnRoot { SisoId = null };

            var actual = intProperty.GetIdValue<NullableGuidOnRoot, Guid>(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetIdValue_WhenGuidNotOnFirstLevel_ThrowsSisoDbException()
        {
            var itemPropertyInfo = typeof(Container).GetProperty("GuidOnRootItem");
            var itemProperty = new Property(itemPropertyInfo);

            var guidPropertyInfo = typeof(GuidOnRoot).GetProperty("SisoId");
            var guidProperty = new Property(1, itemProperty, guidPropertyInfo);

            var item = new Container { GuidOnRootItem = new GuidOnRoot { SisoId = Guid.NewGuid() } };

            var ex = CustomAssert.Throws<SisoDbException>(() => guidProperty.GetIdValue<Container, Guid>(item));

            Assert.AreEqual(ExceptionMessages.Property_GetIdValue_InvalidLevel, ex.Message);
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