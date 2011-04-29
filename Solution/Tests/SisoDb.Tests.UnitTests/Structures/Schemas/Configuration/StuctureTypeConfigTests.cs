using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.Configuration
{
    [TestFixture]
    public class StuctureTypeConfigTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenMissingType_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StructureTypeConfig(null));

            Assert.AreEqual("type", ex.ParamName);
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

            Assert.IsTrue(config.IsEmpty);
        }

        [Test]
        public void IsEmpty_WhenMembersAreRegistrered_ReturnsFalse()
        {
            var config = new StructureTypeConfig(typeof(Dummy));
            config.DoNotIndexThis("Temp");

            Assert.IsFalse(config.IsEmpty);
        }

        [Test]
        public void Generic_IsEmpty_WhenNothingIsRegistrered_ReturnsTrue()
        {
            var config = new StructureTypeConfig<Dummy>();

            Assert.IsTrue(config.IsEmpty);
        }

        [Test]
        public void Generic_IsEmpty_WhenMembersAreRegistrered_ReturnsFalse()
        {
            var config = new StructureTypeConfig<Dummy>();
            config.DoNotIndexThis(x => x.Int1);

            Assert.IsFalse(config.IsEmpty);
        }

        [Test]
        public void OnlyIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsBeingIndexedCollection()
        {
            var config = new StructureTypeConfig(typeof(Dummy));

            config.OnlyIndexThis("Int1", "String1", "Nested.Int1", "Nested.String1");

            var memberPaths = config.MemberPathsBeingIndexed.ToArray();
            Assert.AreEqual(4, memberPaths.Length);
            Assert.AreEqual("Int1", memberPaths[0]);
            Assert.AreEqual("String1", memberPaths[1]);
            Assert.AreEqual("Nested.Int1", memberPaths[2]);
            Assert.AreEqual("Nested.String1", memberPaths[3]);
        }

        [Test]
        public void OnlyIndexThis_WhenCalledWithSameValueTwice_OnlyStoredOnce()
        {
            var config = new StructureTypeConfig(typeof (Dummy));

            config.OnlyIndexThis("Temp", "Temp");

            Assert.AreEqual(1, config.MemberPathsBeingIndexed.Count);
        }

        [Test]
        public void OnlyIndexThis_WhenSpecificNotIndexedMemberPathsExists_MemberPathsNotBeingIndexedGetsCleared()
        {
            var config = new StructureTypeConfig(typeof(Dummy));
            config.DoNotIndexThis("Temp1");

            config.OnlyIndexThis("Temp2");

            Assert.IsNull(config.MemberPathsNotBeingIndexed.SingleOrDefault());
        }

        [Test]
        public void DoNotIndexThis_WhenSpecificOnlyIndexthisMemberPathsExists_MemberPathsBeingIndexedGetsCleared()
        {
            var config = new StructureTypeConfig(typeof(Dummy));
            config.OnlyIndexThis("Temp1");

            config.DoNotIndexThis("Temp2");

            Assert.IsNull(config.MemberPathsBeingIndexed.SingleOrDefault());
        }

        [Test]
        public void DoNotIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsNotBeingIndexedCollection()
        {
            var config = new StructureTypeConfig(typeof(Dummy));

            config.DoNotIndexThis("Int1", "String1", "Nested.Int1", "Nested.String1");

            var memberPaths = config.MemberPathsNotBeingIndexed.ToArray();
            Assert.AreEqual(4, memberPaths.Length);
            Assert.AreEqual("Int1", memberPaths[0]);
            Assert.AreEqual("String1", memberPaths[1]);
            Assert.AreEqual("Nested.Int1", memberPaths[2]);
            Assert.AreEqual("Nested.String1", memberPaths[3]);
        }

        [Test]
        public void Generic_Ctor_TypeIsExtracted_TypeReflectsTheGenericTypeBeingPassed()
        {
            var config = new StructureTypeConfig<Dummy>();

            Assert.AreEqual(typeof(Dummy), config.Type);
        }

        [Test]
        public void Generic_OnlyIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsBeingIndexedCollection()
        {
            var config = new StructureTypeConfig<Dummy>();

            config.OnlyIndexThis(x => x.Int1, x => x.String1, x => x.Nested.Int1, x => x.Nested.String1);

            var memberPaths = config.MemberPathsBeingIndexed.ToArray();
            Assert.AreEqual(4, memberPaths.Length);
            Assert.AreEqual("Int1", memberPaths[0]);
            Assert.AreEqual("String1", memberPaths[1]);
            Assert.AreEqual("Nested.Int1", memberPaths[2]);
            Assert.AreEqual("Nested.String1", memberPaths[3]);
        }

        [Test]
        public void Generic_OnlyIndexThis_WhenCalledWithSameValueTwice_OnlyStoredOnce()
        {
            var config = new StructureTypeConfig<Dummy>();

            config.OnlyIndexThis(x => x.Int1, x => x.Int1);

            Assert.AreEqual(1, config.MemberPathsBeingIndexed.Count);
        }

        [Test]
        public void Generic_OnlyIndexThis_WhenSpecificNotIndexedMemberPathsExists_MemberPathsNotBeingIndexedGetsCleared()
        {
            var config = new StructureTypeConfig<Dummy>();
            config.DoNotIndexThis(x => x.Int1);

            config.OnlyIndexThis(x => x.String1);

            Assert.IsNull(config.MemberPathsNotBeingIndexed.SingleOrDefault());
        }

        [Test]
        public void Generic_DoNotIndexThis_WhenSpecificOnlyIndexthisMemberPathsExists_MemberPathsBeingIndexedGetsCleared()
        {
            var config = new StructureTypeConfig<Dummy>();
            config.OnlyIndexThis(x => x.Int1);

            config.DoNotIndexThis(x => x.String1);

            Assert.IsNull(config.MemberPathsBeingIndexed.SingleOrDefault());
        }

        [Test]
        public void Generic_DoNotIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsNotBeingIndexedCollection()
        {
            var config = new StructureTypeConfig<Dummy>();

            config.DoNotIndexThis(x => x.Int1, x => x.String1, x => x.Nested.Int1, x => x.Nested.String1);

            var memberPaths = config.MemberPathsNotBeingIndexed.ToArray();
            Assert.AreEqual(4, memberPaths.Length);
            Assert.AreEqual("Int1", memberPaths[0]);
            Assert.AreEqual("String1", memberPaths[1]);
            Assert.AreEqual("Nested.Int1", memberPaths[2]);
            Assert.AreEqual("Nested.String1", memberPaths[3]);
        }

        private class Dummy
        {
            public int Int1 { get; set; }

            public string String1 { get; set; }

            public Nested Nested { get; set; }
        }

        private class Nested
        {
            public int Int1 { get; set; }

            public string String1 { get; set; }
        }
    }
}