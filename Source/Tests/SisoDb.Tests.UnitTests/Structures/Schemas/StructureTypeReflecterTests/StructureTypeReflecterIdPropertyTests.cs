using System;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterIdPropertyTests : UnitTestBase
    {
        private readonly IStructureTypeReflecter _reflecter = new StructureTypeReflecter();

        [Test]
        public void GetIdProperty_WhenPublicGuidIdProperty_ReturnsProperty()
        {
            var property = _reflecter.GetIdProperty(typeof (WithGuidId));

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableGuidIdProperty_ReturnsProperty()
        {
            var property = _reflecter.GetIdProperty(typeof (WithNullableGuidId));

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPrivateGuidIdProperty_ReturnsNull()
        {
            var property = _reflecter.GetIdProperty(typeof (WithPrivateGuidId));

            Assert.IsNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPublicIntIdProperty_ReturnsProperty()
        {
            var property = _reflecter.GetIdProperty(typeof (WithIntId));

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableIntIdProperty_ReturnsProperty()
        {
            var property = _reflecter.GetIdProperty(typeof (WithNullableIntId));

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPrivateIntIdProperty_ReturnsNull()
        {
            var property = _reflecter.GetIdProperty(typeof(WithPrivateIntId));

            Assert.IsNull(property);
        }

        private class WithGuidId
        {
            public Guid Id { get; set; }
        }

        private class WithNullableGuidId
        {
            public Guid? Id { get; set; }
        }

        private class WithPrivateGuidId
        {
            private Guid Id { get; set; }
        }

        private class WithIntId
        {
            public int Id { get; set; }
        }

        private class WithNullableIntId
        {
            public int? Id { get; set; }
        }

        private class WithPrivateIntId
        {
            private int Id { get; set; }
        }
    }
}