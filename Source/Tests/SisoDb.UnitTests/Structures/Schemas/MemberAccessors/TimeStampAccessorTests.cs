using System;
using NUnit.Framework;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class TimeStampAccessorTests : UnitTestBase
    {
        private const string TimeStampMemberName = "TimeStamp";

        [Test]
        public void Ctor_WhenMemberIsNotOnRootLevel_ThrowsException()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithMemberNotInRoot>("NestedModelItem.TimeStamp");

            var ex = Assert.Throws<SisoDbException>(() => new TimeStampAccessor(timeStampProperty));

            Assert.AreEqual(ExceptionMessages.TimeStampAccessor_InvalidLevel.Inject(timeStampProperty.Name), ex.Message);
        }

        [Test]
        public void Ctor_WhenMemberIsNotDateTime_ThrowsException()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithStringMember>(TimeStampMemberName);

            var ex = Assert.Throws<SisoDbException>(() => new TimeStampAccessor(timeStampProperty));

            Assert.AreEqual(ExceptionMessages.TimeStampAccessor_Invalid_Type.Inject(timeStampProperty.Name), ex.Message);
        }

        [Test]
        public void GetValue_WhenNoAssignedDateTimeExists_ReturnsMinValueDateTime()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithDateTime>(TimeStampMemberName);
            var accessor = new TimeStampAccessor(timeStampProperty);
            var initialValue = default(DateTime);
            var model = new ModelWithDateTime { TimeStamp = initialValue };

            var timeStamp = accessor.GetValue(model);

            Assert.AreEqual(initialValue, timeStamp);
        }

        [Test]
        public void GetValue_WhenAssignedDateTimeExists_ReturnsAssignedValue()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithDateTime>(TimeStampMemberName);
            var accessor = new TimeStampAccessor(timeStampProperty);
            var initialValue = new DateTime(1970, 12, 13, 01, 02, 03);
            var model = new ModelWithDateTime { TimeStamp = initialValue };

            var timeStamp = accessor.GetValue(model);

            Assert.AreEqual(initialValue, timeStamp);
        }

        [Test]
        public void GetValue_WhenNoAssignedNullableDateTimeExists_ReturnsNulledNullableDateTime()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithNullableDateTime>(TimeStampMemberName);
            var accessor = new TimeStampAccessor(timeStampProperty);
            var model = new ModelWithNullableDateTime { TimeStamp = null };

            var timeStamp = accessor.GetValue(model);

            Assert.IsNull(timeStamp);
        }

        [Test]
        public void GetValue_WhenAssignedNullableDateTimeExists_ReturnsAssignedValue()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithNullableDateTime>(TimeStampMemberName);
            var accessor = new TimeStampAccessor(timeStampProperty);
            var initialValue = new DateTime(1970, 12, 13, 01, 02, 03);
            var model = new ModelWithNullableDateTime { TimeStamp = initialValue };

            var timeStamp = accessor.GetValue(model);

            Assert.AreEqual(initialValue, timeStamp);
        }

        [Test]
        public void SetValue_WhenAssigningDateTime_UpdatesValue()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithDateTime>(TimeStampMemberName);
            var accessor = new TimeStampAccessor(timeStampProperty);
            var initialValue = new DateTime(1970, 12, 13, 01, 02, 03);
            var assignedValue = initialValue.AddDays(1);
            var model = new ModelWithDateTime { TimeStamp = initialValue };

            accessor.SetValue(model, assignedValue);

            Assert.AreEqual(assignedValue, model.TimeStamp);
        }

        [Test]
        public void SetValue_WhenAssigningValueToNullableDateTime_UpdatesValue()
        {
            var timeStampProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithNullableDateTime>(TimeStampMemberName);
            var accessor = new TimeStampAccessor(timeStampProperty);
            var initialValue = new DateTime(1970, 12, 13, 01, 02, 03);
            var assignedValue = initialValue.AddDays(1);
            var model = new ModelWithNullableDateTime { TimeStamp = initialValue };

            accessor.SetValue(model, assignedValue);

            Assert.AreEqual(assignedValue, model.TimeStamp);
        }

        private class ModelWithDateTime
        {
            public DateTime TimeStamp { get; set; }
        }

        private class ModelWithNullableDateTime
        {
            public DateTime? TimeStamp { get; set; }
        }

        private class ModelWithStringMember
        {
            public string TimeStamp { get; set; }
        }

        private class ModelWithMemberNotInRoot
        {
            public ModelWithDateTime NestedModelItem { get; set; }
        }
    }
}