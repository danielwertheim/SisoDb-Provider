using System.Linq;
using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;

namespace SisoDb.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserStringQueryTests : UnitTestBase
    {
        [Test]
        public void Parse_WhenToLower_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.ToLower() == "foo");

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (ToLowerMemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("foo", operandNode.Value);
        }

        [Test]
        public void Parse_WhenToUpper_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.ToUpper() == "foo");

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (ToUpperMemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("foo", operandNode.Value);
        }

        [Test]
        public void Parse_WhenStartsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.StartsWith("Foo"));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("Foo%", operandNode.Value);
        }

        [Test]
        public void Parse_WhenEndsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.EndsWith("bar"));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("%bar", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxToLower_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxToLower() == "foo");

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (ToLowerMemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("foo", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxToUpper_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxToUpper() == "foo");

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (ToUpperMemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("foo", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxStartsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxStartsWith("Foo"));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("Foo%", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxEndsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxEndsWith("bar"));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("%bar", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxLike_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxLike("Foo%bar"));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("Foo%bar", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxContains_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxContains("Foo"));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("%Foo%", operandNode.Value);
        }

        private class MyClass
        {
            public string String1 { get; set; }
        }
    }
}