using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.UnitTests.Structures.Schemas.Configuration
{
    [TestFixture]
    public class StructureTypeConfigurationsTests : UnitTestBase
    {
        [Test]
        public void IsEmpty_WhenNoSpecificConfigurationExists_ReturnsFalse()
        {
            var configs = new StructureTypeConfigurations();

            Assert.IsTrue(configs.IsEmpty);
        }

        [Test]
        public void IsEmpty_WhenSpecificConfigurationExists_ReturnsTrue()
        {
            var configs = new StructureTypeConfigurations();
            configs.Configure<Dummy>(cfg => { });

            Assert.IsFalse(configs.IsEmpty);
        }

        [Test]
        public void Configure_WhenNeverCalledBefore_ConfigurationIsAdded()
        {
            var configs = new StructureTypeConfigurations();

            configs.Configure(typeof(Dummy), cfg => { });

            Assert.AreEqual(1, configs.Items.Count());
        }

        [Test]
        public void Generic_Configure_WhenNeverCalledBefore_ConfigurationIsAdded()
        {
            var configs = new StructureTypeConfigurations();

            configs.Configure<Dummy>(cfg => { });

            Assert.AreEqual(1, configs.Items.Count());
        }

        [Test]
        public void Configure_WhenCalledTwice_StillHasOneConfig()
        {
            var configs = new StructureTypeConfigurations();
            IStructureTypeConfig config1 = null;
            IStructureTypeConfig config2 = null;

            configs.Configure(typeof(Dummy), cfg => { config1 = cfg.Config; });
            configs.Configure(typeof(Dummy), cfg => { config2 = cfg.Config; });

            Assert.AreSame(config1, config2);
            Assert.AreEqual(1, configs.Items.Count());
        }

        [Test]
        public void Generic_Configure_WhenCalledTwice_StillHasOneConfig()
        {
            var configs = new StructureTypeConfigurations();
            IStructureTypeConfig config1 = null;
            IStructureTypeConfig config2 = null;

            configs.Configure<Dummy>(cfg => { config1 = cfg.Config; });
            configs.Configure<Dummy>(cfg => { config2 = cfg.Config; });

            Assert.AreSame(config1, config2);
            Assert.AreEqual(1, configs.Items.Count());
        }

        [Test]
        public void GetConfigurations_WhenRegistreredViaNonGenericVersion_ConfigurationIsReturned()
        {
            var configs = new StructureTypeConfigurations();
            configs.Configure(typeof(Dummy), cfg => { });

            var config = configs.GetConfiguration(typeof(Dummy));

            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(Dummy), config.Type);
        }

        [Test]
        public void GetConfigurations_WhenRegistreredViaGenericVersion_ConfigurationIsReturned()
        {
            var configs = new StructureTypeConfigurations();
            configs.Configure<Dummy>(cfg => { });

            var config = configs.GetConfiguration(typeof(Dummy));

            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(Dummy), config.Type);
        }

        [Test]
        public void Generic_GetConfigurations_WhenRegistreredViaNonGenericVersion_ConfigurationIsReturned()
        {
            var configs = new StructureTypeConfigurations();
            configs.Configure(typeof(Dummy), cfg => { });

            var config = configs.GetConfiguration<Dummy>();

            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(Dummy), config.Type);
        }

        [Test]
        public void Generic_GetConfigurations_WhenRegistreredViaGenericVersion_ConfigurationIsReturned()
        {
            var configs = new StructureTypeConfigurations();
            configs.Configure<Dummy>(cfg => { });

            var config = configs.GetConfiguration<Dummy>();

            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(Dummy), config.Type);
        }

        [Test]
        public void GetConfigurations_WhenNoConfigurationExists_ReturnsNull()
        {
            var configs = new StructureTypeConfigurations();

            var config = configs.GetConfiguration<Dummy>();

            Assert.IsTrue(config.IndexConfigIsEmpty);
            Assert.IsFalse(config.MemberPathsBeingIndexed.Any());
            Assert.IsFalse(config.MemberPathsNotBeingIndexed.Any());
        }

        [Test]
        public void Generic_GetConfigurations_WhenNoConfigurationExists_ReturnsDefaultConfig()
        {
            var configs = new StructureTypeConfigurations();

            var config = configs.GetConfiguration<Dummy>();

            Assert.IsTrue(config.IndexConfigIsEmpty);
            Assert.IsFalse(config.MemberPathsBeingIndexed.Any());
            Assert.IsFalse(config.MemberPathsNotBeingIndexed.Any());
        }

        private class Dummy { }
    }
}