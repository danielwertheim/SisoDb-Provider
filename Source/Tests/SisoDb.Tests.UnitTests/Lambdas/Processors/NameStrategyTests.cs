using NUnit.Framework;
using SisoDb.Lambdas.Processors;

namespace SisoDb.Tests.UnitTests.Lambdas.Processors
{
    [TestFixture]
    public class NameStrategyTests
    {
        private readonly INameStrategy _strategy = new NameStrategy();

        [Test]
        public void Apply_WhenId_TranslatedToStructureId()
        {
            Assert.AreEqual("StructureId", _strategy.Apply("Id"));
        }

        [Test]
        public void Apply_WhenPrefixedWithId_IsNotTranslated()
        {
            Assert.AreEqual("IdTmp", _strategy.Apply("IdTmp"));
        }

        [Test]
        public void Apply_WhenSufixedWithId_IsNotTranslated()
        {
            Assert.AreEqual("TmpId", _strategy.Apply("TmpId"));
        }

        [Test]
        public void Apply_WhenSimulatedNestedId_IsNotTranslated()
        {
            Assert.AreEqual("Child.Id", _strategy.Apply("Child.Id"));
        }
    }
}