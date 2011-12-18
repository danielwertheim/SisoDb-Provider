using System;
using System.Linq;
using NCore;
using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;

namespace SisoDb.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserTests : UnitTestBase
    {
		[Test]
		public void Parse_WhenOnlyMemberIsPassed_ThrowsException()
		{
			var parser = new WhereParser();
			var expression = Reflect<MyClass>.LambdaFrom(m => m.String1);

			var ex = Assert.Throws<SisoDbException>(
				() => parser.Parse(expression));

			Assert.AreEqual(ExceptionMessages.WhereExpressionParser_NoMemberExpressions, ex.Message);
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
		public void Parse_WhenBoolWithoutComparisionOperator_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.Bool1);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(1, listOfNodes.Count);

			var memberNode = (MemberNode)listOfNodes[0];
			Assert.AreEqual("Bool1", memberNode.Path);
			Assert.AreEqual(typeof(bool), memberNode.MemberType);
		}

		[Test]
		public void Parse_WhenNullableIsComparedAgainstValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 == 1);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var operandNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("=", operatorNode.Operator.ToString());
			Assert.AreEqual(1, operandNode.Value);
		}

		[Test]
		public void Parse_WhenNullableValueIsComparedAgainstValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.Value == 1);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var operandNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("=", operatorNode.Operator.ToString());
			Assert.AreEqual(1, operandNode.Value);
		}

		[Test]
		public void Parse_WhenNullableIsNull_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 == null);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("is", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableIsNotNull_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 != null);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstExplicitValueOfFalse_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue == false);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("is", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstExplicitValueOfTrue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue == true);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstImplicitValueOfTrue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstNegatedImplicitValueOfTrue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => !m.NullableInt1.HasValue);

			var parser = new WhereParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(4, listOfNodes.Count);

			var negationNode = (OperatorNode)listOfNodes[0];
			var memberNode = (NullableMemberNode)listOfNodes[1];
			var operatorNode = (OperatorNode)listOfNodes[2];
			var nullNode = (NullNode)listOfNodes[3];

			Assert.AreEqual("not", negationNode.Operator.ToString());
			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.MemberType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

        private class MyClass
        {
            public int Int1 { get; set; }

			public int? NullableInt1 { get; set; }

        	public string String1 { get; set; }

            public bool Bool1 { get; set; }
        }
    }
}