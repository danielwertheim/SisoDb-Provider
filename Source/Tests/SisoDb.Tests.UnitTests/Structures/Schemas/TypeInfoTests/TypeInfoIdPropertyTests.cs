using System;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.TypeInfoTests
{
    [TestFixture]
    public class TypeInfoIdPropertyTests : UnitTestBase
    {
        [Test]
        public void GetIdProperty_WhenPublicGuidIdProperty_ReturnsProperty()
        {
            var property = TypeInfo<WithGuidId>.GetIdProperty();

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableGuidIdProperty_ReturnsProperty()
        {
            var property = TypeInfo<WithNullableGuidId>.GetIdProperty();

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPrivateGuidIdProperty_ReturnsNull()
        {
            var property = TypeInfo<WithPrivateGuidId>.GetIdProperty();

            Assert.IsNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPublicIntIdProperty_ReturnsProperty()
        {
            var property = TypeInfo<WithIntId>.GetIdProperty();

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPublicNullableIntIdProperty_ReturnsProperty()
        {
            var property = TypeInfo<WithNullableIntId>.GetIdProperty();

            Assert.IsNotNull(property);
        }

        [Test]
        public void GetIdProperty_WhenPrivateIntIdProperty_ReturnsNull()
        {
            var property = TypeInfo<WithPrivateIntId>.GetIdProperty();

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