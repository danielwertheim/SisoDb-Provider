using System;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Resources;

namespace SisoDb.Tests.UnitTests.Lambdas
{
    [TestFixture]
    public class LamdaParserTests : UnitTestBase
    {
        [Test]
        public void Parse_WhenOnlyMemberIsPassed_ThrowsException()
        {
            var parser = new LambdaParser();

            var ex = Assert.Throws<SisoDbException>(
                () => parser.Parse<MyClass>(i => i.Bool1));

            Assert.AreEqual(ExceptionMessages.LambdaParser_NoMemberExpressions, ex.Message);
        }

        [Test]
        public void Parse_WhenUnsupportedMethodInvocation_NotSupportedException()
        {
            var parser = new LambdaParser();

            var ex = Assert.Throws<NotSupportedException>(
                () => parser.Parse<MyClass>(i => i.String1 == Convert.ToString(i.Int1)));

            Assert.AreEqual(
                ExceptionMessages.LambdaParser_UnsupportedMethodCall.Inject("ToString(i.Int1)"),
                ex.Message);
        }

        [Test]
        public void Parse_WhenByteArray_NotSupportedException()
        {
            var parser = new LambdaParser();

            var ex = Assert.Throws<NotSupportedException>(
                () => parser.Parse<MyClass>(i => i.Bytes1 == new byte[] { 1, 2 }));

            Assert.AreEqual(ExceptionMessages.LambdaParser_MemberIsBytes.Inject("Bytes1"), ex.Message);
        }

        private class MyClass
        {
            public int Int1 { get; set; }

            public string String1 { get; set; }

            public bool Bool1 { get; set; }

            public byte[] Bytes1 { get; set; }
        }
    }
}