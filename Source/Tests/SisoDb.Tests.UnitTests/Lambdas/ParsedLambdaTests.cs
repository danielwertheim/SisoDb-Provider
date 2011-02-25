using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Tests.UnitTests.Lambdas
{
    [TestFixture]
    public class ParsedLambdaTests : UnitTestBase
    {
        [Test]
        public void MergeAsNew_TwoDifferentSourcesOfDifferentNodes_ReturnsMerged()
        {
            var nodesOne = new List<INode>{new TestNode(1)};
            var nodesTwo = new List<INode> { new TestNode(1) };

            var parsedLambdaOne = new ParsedLambda(nodesOne);
            var parsedLambdaTwo = new ParsedLambda(nodesTwo);

            var mergedParsedLambda = parsedLambdaOne.MergeAsNew(parsedLambdaTwo);

            CollectionAssert.AreEquivalent(nodesOne.Union(nodesTwo), mergedParsedLambda.Nodes);
        }

        [Test]
        public void MergeAsNew_SameSourcesOfNodes_ReturnsMerged()
        {
            var nodesOne = new List<INode> { new TestNode(1), new TestNode(2) };
            
            var parsedLambdaOne = new ParsedLambda(nodesOne);
            var parsedLambdaTwo = new ParsedLambda(nodesOne);

            var mergedParsedLambda = parsedLambdaOne.MergeAsNew(parsedLambdaTwo);

            CollectionAssert.AreEquivalent(nodesOne, mergedParsedLambda.Nodes);
        }

        private class TestNode : INode
        {
            public int Value { get; private set; }

            public TestNode(int value)
            {
                Value = value;
            }
        }
    }
}