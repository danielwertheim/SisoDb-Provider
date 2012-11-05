using System;
using NUnit.Framework;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.UnitTests.Structures.Schemas.Configuration
{
    [TestFixture]
    public class StructureTypeConfigTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenMissingType_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StructureTypeConfig(null));

            Assert.AreEqual("structureType", ex.ParamName);
        }

        [Test]
        public void Ctor_WhenPassingType_TypePropertyIsAssigned()
        {
            var expectedType = typeof (Dummy);
            var config = new StructureTypeConfig(expectedType);

            Assert.AreEqual(expectedType, config.Type);
        }

        [Test]
        public void IsEmpty_WhenNothingIsRegistrered_ReturnsTrue()
        {
            var config = new StructureTypeConfig(typeof (Dummy));

            Assert.IsTrue(config.IndexConfigIsEmpty);
        }

        [Test]
        public void IsEmpty_WhenMembersAreExcluded_ReturnsFalse()
        {
            var config = new StructureTypeConfig(typeof(Dummy));
            config.MemberPathsNotBeingIndexed.Add("Temp");

            Assert.IsFalse(config.IndexConfigIsEmpty);
        }

        [Test]
        public void IsEmpty_WhenMembersAreIncluded_ReturnsFalse()
        {
            var config = new StructureTypeConfig(typeof(Dummy));
            config.MemberPathsBeingIndexed.Add("Temp");

            Assert.IsFalse(config.IndexConfigIsEmpty);
        }

        [Test]
        public void IncludeContainedStructureMembers_WhenDefault_Ctor_IsFalse()
        {
            var config = new StructureTypeConfig(typeof(Dummy));

            Assert.IsFalse(config.IncludeContainedStructureMembers);
        }

        private class Dummy {}
    }
}