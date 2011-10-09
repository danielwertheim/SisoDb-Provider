using System;
using NCore;
using NUnit.Framework;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserTests : UnitTestBase
    {
        [Test]
        public void Parse_WhenOnlyMemberIsPassed_ThrowsException()
        {
            var parser = new WhereParser();
            var expression = Reflect<MyClass>.LambdaFrom(m => m.Bool1);

            var ex = Assert.Throws<SisoDbException>(
                () => parser.Parse(expression));

            Assert.AreEqual(ExceptionMessages.LambdaParser_NoMemberExpressions, ex.Message);
        }

        [Test]
        public void Parse_WhenUnsupportedMethodInvocation_NotSupportedException()
        {
            var parser = new WhereParser();
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1 == Convert.ToString(m.Int1));

            var ex = Assert.Throws<NotSupportedException>(
                () => parser.Parse(expression));

            Assert.AreEqual(
                ExceptionMessages.LambdaParser_UnsupportedMethodCall.Inject("ToString(m.Int1)"),
                ex.Message);
        }

        [Test]
        public void Parse_WhenByteArray_NotSupportedException()
        {
            var parser = new WhereParser();
            var expression = Reflect<MyClass>.LambdaFrom(i => i.Bytes1 == new byte[] {1, 2});

            var ex = Assert.Throws<NotSupportedException>(
                () => parser.Parse(expression));

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