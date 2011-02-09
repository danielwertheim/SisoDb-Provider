using NUnit.Framework;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Tests.UnitTests.Lambdas.Nodes
{
    [TestFixture]
    public class NameStrategyTests
    {
        private NameStrategy _strategy = new NameStrategy();

        [Test]
        public void Apply_WhenId_TranslatedToStructureId()
        {
            Assert.AreEqual("StructureId", _strategy.Apply("Id"));
        }

        [Test]
        public void Apply_WhenNotId_IsNotTranslated()
        {
            Assert.AreEqual("Id1", _strategy.Apply("Id1"));
        }
    }
}