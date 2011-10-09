using System;
using NUnit.Framework;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class SimpleMemberPathGeneratorTests : UnitTestBase
    {
        private IMemberPathGenerator _generator;

        protected override void OnTestInitialize()
        {
            _generator = new SimpleMemberPathGenerator();
        }

        [Test]
        public void Generate_WhenPassedValueIsSisoId_NoHashIsAppended()
        {
            var inputValue = "SisoId";
            var memberName = _generator.Generate(inputValue);

            Assert.AreEqual(inputValue, memberName);
        }

        [Test]
        public void Generate_WhenCalledWithNull_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _generator.Generate(null));

            Assert.AreEqual("memberPath", ex.ParamName);
        }

        [Test]
        public void Generate_WhenCalledWithWhiteSpaceString_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _generator.Generate("   "));

            Assert.AreEqual("memberPath", ex.ParamName);
        }
    }
}