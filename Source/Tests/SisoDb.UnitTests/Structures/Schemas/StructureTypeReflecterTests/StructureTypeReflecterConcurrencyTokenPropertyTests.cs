using System;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructureTypeReflecterTests
{
    [TestFixture]
    public class StructureTypeReflecterConcurrencyTokenPropertyTests : StructureTypeReflecterTestsBase
    {
        [Test]
        public void HasConcurrencyTokenProperty_WhenMemberExists_ReturnsTrue()
        {
            Assert.IsTrue(ReflecterFor().HasConcurrencyTokenProperty(typeof(Model)));
        }

        [Test]
        public void HasConcurrencyTokenProperty_WhenMemberDoesNotExists_ReturnsFalse()
        {
            Assert.IsFalse(ReflecterFor().HasConcurrencyTokenProperty(typeof(ModelWithNoToken)));
        }

        [Test]
        public void GetConcurrencyTokenProperty_WhenMemberExists_ReturnsProperty()
        {
            var property = ReflecterFor().GetConcurrencyTokenProperty(typeof(Model));

            Assert.IsNotNull(property);
            Assert.AreEqual("ConcurrencyToken", property.Name);
        }

        [Test]
        public void GetConcurrencyTokenProperty_WhenMemberDoesNotExist_ReturnsNull()
        {
            var property = ReflecterFor().GetConcurrencyTokenProperty(typeof(ModelWithNoToken));

            Assert.IsNull(property);
        }

        private class Model
        {
            public Guid ConcurrencyToken { get; set; }
        }

        private class ModelWithNoToken
        {

        }
    }
}