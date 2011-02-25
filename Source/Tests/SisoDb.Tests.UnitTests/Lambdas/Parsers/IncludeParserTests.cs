using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Lambdas.Nodes;
using SisoDb.Lambdas.Parsers;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests.Lambdas.Parsers
{
    [TestFixture]
    public class IncludeParserTests : UnitTestBase
    {
        [Test]
        public void Parse_WhenNullExpressions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new IncludeParser().Parse<ChildWithGuidId>(null));
        }

        [Test]
        public void Parse_WhenEmptyExpressions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new IncludeParser().Parse<ChildWithGuidId>(new LambdaExpression[0]));
        }

        [Test]
        public void Parse_WhenNullExpression_DoesNotThrowNullReferenceException()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            Assert.DoesNotThrow(
                () => new SortingParser().Parse(new[] { lambda, null }));
        }

        [Test]
        public void Parse_WhenMemberExpressionIsPassed_GeneratesExactlyOneIncludeNode()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            var parser = new IncludeParser();
            var parsedLambda = parser.Parse<ChildWithGuidId>(new[] { lambda });

            Assert.AreEqual(1, parsedLambda.Nodes.Count);
            Assert.IsNotNull(parsedLambda.Nodes[0] as IncludeNode);
        }

        [Test]
        public void Parse_WhenMemberExpressionIsPassed_HasCorrectParentMemberNode()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            var parser = new IncludeParser();
            var parsedLambda = parser.Parse<ChildWithGuidId>(new[] { lambda });
            var includeNode = (IncludeNode)parsedLambda.Nodes[0];

            Assert.AreEqual("ChildId", includeNode.IdReferencePath);
        }

        [Test]
        public void Parse_WhenNestedMemberExpressionIsPassed_HasCorrectParentMemberNode()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.Child.GrandChild.Id);

            var parser = new IncludeParser();
            var parsedLambda = parser.Parse<ChildWithGuidId>(new[] { lambda });
            var includeNode = (IncludeNode)parsedLambda.Nodes[0];

            Assert.AreEqual("Child.GrandChild.Id", includeNode.IdReferencePath);
        }

        [Test]
        public void Parse_WhenMemberExpressionIsPassed_HasCorrectChildStructureName()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            var parser = new IncludeParser();
            var parsedLambda = parser.Parse<ChildWithGuidId>(new[] { lambda });
            var includeNode = (IncludeNode)parsedLambda.Nodes[0];

            Assert.AreEqual("ChildWithGuidId", includeNode.ChildStructureName);
        }

        private class Master
        {
            public Guid Id { get; set; }

            public Guid ChildId { get; set; }

            public Child Child { get; set; }
        }

        private class ChildWithGuidId
        {
            public Guid Id { get; set; }
        }

        private class Child
        {
            public GrandChild GrandChild { get; set; }
        }

        private class GrandChild
        {
            public Guid Id { get; set; }
        }
    }
}