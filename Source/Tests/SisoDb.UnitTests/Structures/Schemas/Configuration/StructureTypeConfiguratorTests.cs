using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.UnitTests.Structures.Schemas.Configuration
{
    [TestFixture]
    public class StructureTypeConfiguratorTests : UnitTestBase
    {
        private IStructureTypeConfig UseNonGenericConfiguratorFor<T>(Action<IStructureTypeConfigurator> configure) where T : class
        {
            var config = new StructureTypeConfig(typeof(T));
            var configurator = new StructureTypeConfigurator(config);

            configure(configurator);

            return config;
        }

        private IStructureTypeConfig UseGenericConfiguratorFor<T>(Action<IStructureTypeConfigurator<T>> configure) where T : class
        {
            var config = new StructureTypeConfig(typeof(T));
            var configurator = new StructureTypeConfigurator<T>(config);

            configure(configurator);

            return config;
        }

        [Test]
        public void OnlyIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsBeingIndexedCollection()
        {
            var config = UseNonGenericConfiguratorFor<Dummy>(cfg => cfg.OnlyIndexThis("Int1", "String1", "Nested.Int1", "Nested.String1"));

            var memberPaths = config.MemberPathsBeingIndexed.ToArray();
            Assert.AreEqual(4, memberPaths.Length);
            Assert.AreEqual("Int1", memberPaths[0]);
            Assert.AreEqual("String1", memberPaths[1]);
            Assert.AreEqual("Nested.Int1", memberPaths[2]);
            Assert.AreEqual("Nested.String1", memberPaths[3]);
        }

        [Test]
        public void Generic_OnlyIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsBeingIndexedCollection()
        {
            var config = UseGenericConfiguratorFor<Dummy>(cfg => cfg.OnlyIndexThis(x => x.Int1, x => x.String1, x => x.Nested.Int1, x => x.Nested.String1));

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
            var config = UseNonGenericConfiguratorFor<Dummy>(cfg => cfg.OnlyIndexThis("String1", "String1"));

            Assert.AreEqual(1, config.MemberPathsBeingIndexed.Count);
        }

        [Test]
        public void Generic_OnlyIndexThis_WhenCalledWithSameValueTwice_OnlyStoredOnce()
        {
            var config = UseGenericConfiguratorFor<Dummy>(cfg => cfg.OnlyIndexThis(x => x.String1, x => x.String1));

            Assert.AreEqual(1, config.MemberPathsBeingIndexed.Count);
        }

        [Test]
        public void OnlyIndexThis_WhenSpecificNotIndexedMemberPathsExists_MemberPathsNotBeingIndexedGetsCleared()
        {
            var config = UseNonGenericConfiguratorFor<Dummy>(cfg =>
            {
                cfg.DoNotIndexThis("Int1");
                cfg.OnlyIndexThis("String1");
            });

            Assert.IsFalse(config.MemberPathsNotBeingIndexed.Any());
        }

        [Test]
        public void Generic_OnlyIndexThis_WhenSpecificNotIndexedMemberPathsExists_MemberPathsNotBeingIndexedGetsCleared()
        {
            var config = UseGenericConfiguratorFor<Dummy>(cfg =>
            {
                cfg.DoNotIndexThis(x => x.Int1);
                cfg.OnlyIndexThis(x => x.String1);
            });

            Assert.IsFalse(config.MemberPathsNotBeingIndexed.Any());
        }

        [Test]
        public void DoNotIndexThis_WhenSpecificOnlyIndexthisMemberPathsExists_MemberPathsBeingIndexedGetsCleared()
        {
            var config = UseNonGenericConfiguratorFor<Dummy>(cfg =>
            {
                cfg.OnlyIndexThis("String1");
                cfg.DoNotIndexThis("Int1");
            });

            Assert.IsFalse(config.MemberPathsBeingIndexed.Any());
        }

        [Test]
        public void Generic_DoNotIndexThis_WhenSpecificOnlyIndexthisMemberPathsExists_MemberPathsBeingIndexedGetsCleared()
        {
            var config = UseGenericConfiguratorFor<Dummy>(cfg =>
            {
                cfg.OnlyIndexThis(x => x.String1);
                cfg.DoNotIndexThis(x => x.Int1);
            });

            Assert.IsFalse(config.MemberPathsBeingIndexed.Any());
        }

        [Test]
        public void DoNotIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsNotBeingIndexedCollection()
        {
            var config = UseNonGenericConfiguratorFor<Dummy>(cfg => cfg.DoNotIndexThis("Int1", "String1", "Nested.Int1", "Nested.String1"));

            var memberPaths = config.MemberPathsNotBeingIndexed.ToArray();
            Assert.AreEqual(4, memberPaths.Length);
            Assert.AreEqual("Int1", memberPaths[0]);
            Assert.AreEqual("String1", memberPaths[1]);
            Assert.AreEqual("Nested.Int1", memberPaths[2]);
            Assert.AreEqual("Nested.String1", memberPaths[3]);
        }

        [Test]
        public void Generic_DoNotIndexThis_WhenFirstTimeCalled_MemberPathsIsStoredIn_MemberPathsNotBeingIndexedCollection()
        {
            var config = UseGenericConfiguratorFor<Dummy>(cfg => cfg.DoNotIndexThis(x => x.Int1, x => x.String1, x => x.Nested.Int1, x => x.Nested.String1));

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