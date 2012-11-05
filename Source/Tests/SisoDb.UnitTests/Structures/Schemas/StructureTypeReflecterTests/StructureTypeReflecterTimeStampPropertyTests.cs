using System;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterTimeStampPropertyTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void HasTimeStampProperty_WhenMemberExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasTimeStampProperty(typeof(Model)));
        }

        [Test]
        public void HasTimeStampProperty_WhenClassNamedMemberExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasTimeStampProperty(typeof(ModelWithModelTimeStamp)));
        }

        [Test]
        public void HasTimeStampProperty_WhenInterfaceNamedMemberExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasTimeStampProperty(typeof(IModel)));
        }

        [Test]
        public void HasTimeStampProperty_WhenStructureNamedMemberExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasTimeStampProperty(typeof(ModelWithStructureTimeStamp)));
        }

        [Test]
        public void HasTimeStampProperty_WhenModelWithNullableMemberExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasTimeStampProperty(typeof(ModelWithNullableTimeStamp)));
        }

        [Test]
        public void HasTimeStampProperty_WhenMemberDoesNotExists_ReturnsFalse()
        {
            Assert.IsFalse(ReflecterFor().HasTimeStampProperty(typeof(ModelWithNoTimeStamp)));
        }

        [Test]
        public void GetTimeStampProperty_WhenMemberExists_ReturnsProperty()
        {
            var property = ReflecterFor().GetTimeStampProperty(typeof(Model));

            Assert.IsNotNull(property);
            Assert.AreEqual("TimeStamp", property.Name);
        }

        [Test]
        public void GetTimeStampProperty_WhenMemberDoesNotExist_ReturnsNull()
        {
            var property = ReflecterFor().GetTimeStampProperty(typeof(ModelWithNoTimeStamp));

            Assert.IsNull(property);
        }

        private class Model
        {
            public DateTime TimeStamp { get; set; }
        }

        private class ModelWithNullableTimeStamp
        {
            public DateTime? TimeStamp { get; set; }
        }

        private class ModelWithStructureTimeStamp
        {
            public DateTime StructureTimeStamp { get; set; }
        }

        private class ModelWithModelTimeStamp
        {
            public DateTime ModelWithModelTimeStampTimeStamp { get; set; }
        }

        private interface IModel
        {
            DateTime ModelTimeStamp { get; set; }
        }

        private class ModelWithNoTimeStamp
        {

        }
    }
}