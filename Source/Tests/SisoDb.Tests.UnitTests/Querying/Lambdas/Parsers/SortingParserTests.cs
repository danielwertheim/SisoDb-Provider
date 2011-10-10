using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class SortingParserTests : UnitTestBase
    {
        [Test]
        public void Parse_WhenNonMemberExpression_ThrowsSisoDbException()
        {
            var parser = new SortingParser();
            var nonMemberExpression = Reflect<MyClass>.BoolExpressionFrom(m => m.Int1 == 32);

            var ex = Assert.Throws<SisoDbException>(() => parser.Parse(new []{nonMemberExpression}));

            Assert.AreEqual("No MemberExpression found in expression: '(m.Int1 == 32)'.", ex.Message);
        }

        [Test]
        public void Parse_WhenNullExpressions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new SortingParser().Parse(null));
        }

        [Test]
        public void Parse_WhenEmptyExpressions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new SortingParser().Parse(new LambdaExpression[]{}));
        }

        [Test]
        public void Parse_WhenNullExpression_DoesNotThrowNullReferenceException()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1);

            Assert.DoesNotThrow(
                () => new SortingParser().Parse(new LambdaExpression[] { lambda, null }));
        }

        [Test]
        public void Parse_WhenNestedMemberIsMethod_ThrowsNotSupportedException()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.Child.DoSomething());

            var ex = Assert.Throws<NotSupportedException>(
                () => new SortingParser().Parse(new[] { lambda }));

            Assert.AreEqual(ExceptionMessages.SortingParser_UnsupportedMethodForSortingDirection, ex.Message);
        }

        [Test]
        public void Parse_WhenOnePropertyLambdaIsPassed_ExactlyOneSortingNodeIsReturned()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1);

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            Assert.AreEqual(1, parsedLambda.Nodes.Count);
            Assert.IsInstanceOf(typeof(SortingNode), parsedLambda.Nodes[0]);
        }

        [Test]
        public void Parse_WhenTwoPropertyLambdasArePassed_ExactlyTwoSortingNodeIsReturned()
        {
            var lambda1 = Reflect<MyClass>.LambdaFrom(m => m.String1);
            var lambda2 = Reflect<MyClass>.LambdaFrom(m => m.Int1);

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda1, lambda2 });

            Assert.AreEqual(2, parsedLambda.Nodes.Count);
            Assert.IsInstanceOf(typeof(SortingNode), parsedLambda.Nodes[0]);
            Assert.IsInstanceOf(typeof(SortingNode), parsedLambda.Nodes[1]);
        }

        [Test]
        public void Parse_WhenFirstLevelLambda_MemberPathIsPropertyName()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1);

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual("String1", node.MemberPath);
        }

        [Test]
        public void Parse_WhenNestedLambda_MemberPathReflectsHierarchy()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.Child.String1);

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual("Child.String1", node.MemberPath);
        }

        [Test]
        public void Parse_WhenAscExtension_SortingDirectionBecomesAsc()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1.Asc());

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual(SortDirections.Asc, node.Direction);
        }

        [Test]
        public void Parse_WhenAscExtensionOnNested_SortingDirectionBecomesAsc()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.Child.String1.Asc());

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual(SortDirections.Asc, node.Direction);
        }

        [Test]
        public void Parse_WhenAscExtensionOnNested_MemberPathReflectsHierarchy()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.Child.String1.Asc());

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual("Child.String1", node.MemberPath);
        }

        [Test]
        public void Parse_WhenNeitherAscNorDescExtension_SortingDirectionBecomesAsc()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1);

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual(SortDirections.Asc, node.Direction);
        }

        [Test]
        public void Parse_WhenDescExtension_SortingDirectionBecomesDesc()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1.Desc());

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual(SortDirections.Desc, node.Direction);
        }

        [Test]
        public void Parse_WhenAscAndDescExtensionOnSameMember_SortingDirectionBecomesDesc()
        {
            var lambda = Reflect<MyClass>.LambdaFrom(m => m.String1.Asc().Desc());

            var parsedLambda = new SortingParser()
                .Parse(new[] { lambda });

            var node = (SortingNode)parsedLambda.Nodes[0];
            Assert.AreEqual(SortDirections.Desc, node.Direction);
        }

        private class MyClass
        {
            public string String1 { get; set; }

            public int Int1 { get; set; }

            public MyChildClass Child { get; set; }
        }

        private class MyChildClass
        {
            public string String1 { get; set; }

            public void DoSomething()
            { }
        }
    }
}