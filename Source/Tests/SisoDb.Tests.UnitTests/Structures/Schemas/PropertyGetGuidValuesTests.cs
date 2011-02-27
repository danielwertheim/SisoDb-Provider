using System;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class PropertyGetGuidValuesTests : UnitTestBase
    {
        [Test]
        public void GetIdValue_WhenGuidOnFirstLevel_ReturnsGuid()
        {
            var guidPropertyInfo = typeof(Dummy2).GetProperty("Id");
            var guidProperty = new Property(guidPropertyInfo);

            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new Dummy2 { Id = expected };

            var actual = guidProperty.GetIdValue<Guid>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullableGuidOnFirstLevel_ReturnsGuid()
        {
            var intPropertyInfo = typeof(Dummy2).GetProperty("NullableId");
            var intProperty = new Property(intPropertyInfo);

            var expected = Guid.Parse("4217F3B7-6DEB-4DFA-B195-D111C1297988");
            var item = new Dummy2 { NullableId = expected };

            var actual = intProperty.GetIdValue<Guid>(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetIdValue_WhenNullAssignedNullableGuidOnFirstLevel_ReturnsNull()
        {
            var intPropertyInfo = typeof(Dummy2).GetProperty("NullableId");
            var intProperty = new Property(intPropertyInfo);

            var item = new Dummy2 { NullableId = null };

            var actual = intProperty.GetIdValue<Guid>(item);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetIdValue_WhenGuidNotOnFirstLevel_ThrowsSisoDbException()
        {
            var dummy2PropertyInfo = typeof (Dummy1).GetProperty("Item1");
            var dummy2Property = new Property(dummy2PropertyInfo);

            var guidPropertyInfo = typeof(Dummy2).GetProperty("Id");
            var guidProperty = new Property(1, dummy2Property,guidPropertyInfo);

            var item = new Dummy1 { Item1 = new Dummy2 { Id = Guid.NewGuid() } };

            var ex = CustomAssert.Throws<SisoDbException>(() => guidProperty.GetIdValue<Guid>(item));

            Assert.AreEqual(ExceptionMessages.Property_GetIdValue_InvalidLevel, ex.Message);
        }

        public class Dummy1
        {
            public Dummy2 Item1 { get; set; }
        }

        public class Dummy2
        {
            public Guid Id { get; set; }

            public Guid? NullableId { get; set; }
        }
    }
}