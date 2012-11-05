using System;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterIdPropertyTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void HasIdProperty_WhenGuidIdPropertyExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(WithGuidId)));
        }

        [Test]
        public void HasIdProperty_WhenIdentityPropertyExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(WithIntId)));
        }

        [Test]
        public void HasIdProperty_WhenNullableGuidIdPropertyExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(WithNullableGuidId)));
        }

        [Test]
        public void HasIdProperty_WhenNullableIdentityPropertyExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(WithNullableIntId)));
        }

        [Test]
        public void HasIdProperty_WhenIdPropertyDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(ReflecterFor().HasIdProperty(typeof(WithNoId)));
        }

		[Test]
		public void HasIdProperty_WhenIdPropertyNameIsTypeNamedId_ReturnsTrue()
		{
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(WithCustomIdOfTypeName)));
		}

		[Test]
		public void HasIdProperty_WhenIdPropertyNameIsInterfaceNamedId_ReturnsTrue()
		{
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(IMyType)));
		}

		[Test]
		public void HasIdProperty_WhenIdPropertyNameIsId_ReturnsTrue()
		{
            Assert.IsTrue(ReflecterFor().HasIdProperty(typeof(WithId)));
		}

        [Test]
        public void GetIdProperty_WhenPublicGuidIdProperty_ReturnsProperty()
        {
            Assert.IsNotNull(ReflecterFor().GetIdProperty(typeof(WithGuidId)));
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableGuidIdProperty_ReturnsProperty()
        {
            Assert.IsNotNull(ReflecterFor().GetIdProperty(typeof(WithNullableGuidId)));
        }

        [Test]
        public void GetIdProperty_WhenPrivateGuidIdProperty_ReturnsNull()
        {
            Assert.IsNull(ReflecterFor().GetIdProperty(typeof(WithPrivateGuidId)));
        }

        [Test]
        public void GetIdProperty_WhenPublicIntIdProperty_ReturnsProperty()
        {
            Assert.IsNotNull(ReflecterFor().GetIdProperty(typeof(WithIntId)));
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableIntIdProperty_ReturnsProperty()
        {
            Assert.IsNotNull(ReflecterFor().GetIdProperty(typeof(WithNullableIntId)));
        }

        [Test]
        public void GetIdProperty_WhenPrivateIntIdProperty_ReturnsNull()
        {
            Assert.IsNull(ReflecterFor().GetIdProperty(typeof(WithPrivateIntId)));
        }

        [Test]
        public void GetIdProperty_WhenPublicLongIdProperty_ReturnsProperty()
        {
            Assert.IsNotNull(ReflecterFor().GetIdProperty(typeof(WithLongId)));
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableLongIdProperty_ReturnsProperty()
        {
            Assert.IsNotNull(ReflecterFor().GetIdProperty(typeof(WithNullableLongId)));
        }

        [Test]
        public void GetIdProperty_WhenPrivateLongIdProperty_ReturnsNull()
        {
            Assert.IsNull(ReflecterFor().GetIdProperty(typeof(WithPrivateLongId)));
        }

		[Test]
		public void GetIdProperty_WhenIdPropertyNameIsTypeNamedId_ReturnsProperty()
		{
            var property = ReflecterFor().GetIdProperty(typeof(WithCustomIdOfTypeName));

			Assert.IsNotNull(property);
			Assert.AreEqual("WithCustomIdOfTypeNameId", property.Name);
		}

		[Test]
		public void GetIdProperty_WhenIdPropertyNameIsInterfaceNamedId_ReturnsProperty()
		{
            var property = ReflecterFor().GetIdProperty(typeof(IMyType));

			Assert.IsNotNull(property);
			Assert.AreEqual("MyTypeId", property.Name);
		}

		[Test]
		public void GetIdProperty_WhenIdPropertyNameIsId_ReturnsProperty()
		{
            var property = ReflecterFor().GetIdProperty(typeof(WithId));

			Assert.IsNotNull(property);
			Assert.AreEqual("Id", property.Name);
		}

        [Test]
        public void GetIndexableProperties_WhenGuidIdExists_IdMemberIsReturned()
        {
            var property = ReflecterFor().GetIndexableProperties(typeof(WithGuidId))
                .SingleOrDefault(p => p.Path == "StructureId");

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIndexableProperties_WhenIntIdExists_IdMemberIsReturned()
        {
            var property = ReflecterFor().GetIndexableProperties(typeof(WithIntId))
                .SingleOrDefault(p => p.Path == "StructureId");

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIndexableProperties_WhenLongIdExists_IdMemberIsReturned()
        {
            var property = ReflecterFor().GetIndexableProperties(typeof(WithLongId))
                .SingleOrDefault(p => p.Path == "StructureId");

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIndexableProperties_WhenNulledNullableGuidIdExists_IdMemberIsReturned()
        {
            var property = ReflecterFor().GetIndexableProperties(typeof(WithNullableGuidId))
                .SingleOrDefault(p => p.Path == "StructureId");

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIndexableProperties_WhenNullableIntExists_IdMemberIsReturned()
        {
            var property = ReflecterFor().GetIndexableProperties(typeof(WithNullableIntId))
                .SingleOrDefault(p => p.Path == "StructureId");

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIndexableProperties_WhenNullableLongExists_IdMemberIsReturned()
        {
            var property = ReflecterFor().GetIndexableProperties(typeof(WithNullableLongId))
                .SingleOrDefault(p => p.Path == "StructureId");

            Assert.IsNotNull(property);
        }

        private class WithNoId
        {}

        private class WithGuidId
        {
            public Guid StructureId { get; set; }
        }

        private class WithNullableGuidId
        {
            public Guid? StructureId { get; set; }
        }

        private class WithPrivateGuidId
        {
            private Guid StructureId { get; set; }
        }

        private class WithIntId
        {
            public int StructureId { get; set; }
        }

        private class WithNullableIntId
        {
            public int? StructureId { get; set; }
        }

        private class WithPrivateIntId
        {
            private int StructureId { get; set; }
        }

        private class WithLongId
        {
            public long StructureId { get; set; }
        }

        private class WithNullableLongId
        {
            public long? StructureId { get; set; }
        }

        private class WithPrivateLongId
        {
            private long StructureId { get; set; }
        }

		private class WithCustomIdOfTypeName
		{
			public Guid WithCustomIdOfTypeNameId { get; set; }
		}

		private interface IMyType
		{
			Guid MyTypeId { get; set; } 
		}

		private class WithId
		{
			public Guid Id { get; set; }
		}
    }
}