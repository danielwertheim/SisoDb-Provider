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
        public void Generate_WhenPassedValueIsStructureId_NoHashIsAppended()
        {
            var inputValue = "StructureId";
            var memberName = _generator.Generate(inputValue);

            Assert.AreEqual(inputValue, memberName);
        }

        [Test]
        public void Generate_WhenCalledWithNull_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => _generator.Generate(null));

            Assert.AreEqual("memberPath", ex.ParamName);
        }

        [Test]
        public void Generate_WhenCalledWithWhiteSpaceString_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => _generator.Generate("   "));

            Assert.AreEqual("memberPath", ex.ParamName);
        }
    }
}