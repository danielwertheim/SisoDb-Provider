using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.NCore;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserTests : UnitTestBase
    {
        private readonly DataTypeConverter _dataTypeConverter = new DataTypeConverter();

        private WhereParser CreateParser()
        {
            return new WhereParser(_dataTypeConverter);
        }

		[Test]
		public void Parse_WhenOnlyMemberIsPassed_ThrowsException()
		{
			var parser = CreateParser();
			var expression = Reflect<MyClass>.LambdaFrom(m => m.String1);

			var ex = Assert.Throws<SisoDbException>(
				() => parser.Parse(expression));

			Assert.AreEqual(ExceptionMessages.WhereExpressionParser_NoMemberExpressions, ex.Message);
		}

		[Test]
		public void Parse_WhenUnsupportedMethodInvocation_NotSupportedException()
		{
			var parser = CreateParser();
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

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (MemberNode)listOfNodes[0];
			var equalNode = (OperatorNode) listOfNodes[1];
			var valueNode = (ValueNode) listOfNodes[2];

			Assert.AreEqual("Bool1", memberNode.Path);
			Assert.AreEqual(typeof(bool), memberNode.DataType);
			Assert.AreEqual("=", equalNode.ToString());
			Assert.AreEqual(true, valueNode.Value);
		}

		[Test]
		public void Parse_WhenBoolWithComparisionOperatorAgainstVariabel_ReturnsCorrectNodes()
		{
			var item = new MyClass {Bool1 = false};
			var expression = Reflect<MyClass>.LambdaFrom(m => m.Bool1 == item.Bool1);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (MemberNode)listOfNodes[0];
			var equalNode = (OperatorNode)listOfNodes[1];
			var valueNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("Bool1", memberNode.Path);
			Assert.AreEqual(typeof(bool), memberNode.DataType);
			Assert.AreEqual("=", equalNode.ToString());
			Assert.AreEqual(false, valueNode.Value);
		}

		[Test]
		public void Parse_WhenBoolWithComparisionOperator_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.Bool1 == true);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (MemberNode)listOfNodes[0];
			var equalNode = (OperatorNode)listOfNodes[1];
			var valueNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("Bool1", memberNode.Path);
			Assert.AreEqual(typeof(bool), memberNode.DataType);
			Assert.AreEqual("=", equalNode.ToString());
			Assert.AreEqual(true, valueNode.Value);
		}

		[Test]
		public void Parse_WhenBoolWithoutComparisionOperatorAndWithNullable_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.Bool1 || m.NullableInt1.HasValue);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(7, listOfNodes.Count);

			var memberNode1 = (MemberNode)listOfNodes[0];
			var equalNode = (OperatorNode)listOfNodes[1];
			var valueNode = (ValueNode)listOfNodes[2];
			var orNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (NullableMemberNode)listOfNodes[4];
			var isNotNode2 = (OperatorNode)listOfNodes[5];
			var nullNode2 = (NullNode)listOfNodes[6];
			
			Assert.AreEqual("Bool1", memberNode1.Path);
			Assert.AreEqual(typeof(bool), memberNode1.DataType);
			Assert.AreEqual("=", equalNode.Operator.ToString());
			Assert.AreEqual(true, valueNode.Value);
			Assert.AreEqual("or", orNode.ToString());
			Assert.AreEqual("NullableInt1", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("is not", isNotNode2.Operator.ToString());
			Assert.AreEqual("null", nullNode2.ToString());
		}

		[Test]
		public void Parse_WhenCombiningNullableHasValueAndValueInExpression_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue && m.NullableInt1 != null);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(9, listOfNodes.Count);

			var startGroupNode = (StartGroupNode) listOfNodes[0];
			var memberNode1 = (NullableMemberNode)listOfNodes[1];
			var isNotNode1 = (OperatorNode)listOfNodes[2];
			var nullNode1 = (NullNode)listOfNodes[3];
			var andNode = (OperatorNode) listOfNodes[4];
			var memberNode2 = (NullableMemberNode)listOfNodes[5];
			var isNotNode2 = (OperatorNode)listOfNodes[6];
			var nullNode2 = (NullNode)listOfNodes[7];
			var endGroupNode = (EndGroupNode) listOfNodes[8];

			Assert.AreEqual("NullableInt1", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt1", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("is not", isNotNode2.Operator.ToString());
			Assert.AreEqual("null", nullNode2.ToString());
		}

		[Test]
    	public void Parse_WhenNullableIsComparedAgainstNullableVariableWithValue_ReturnsCorrectNodes()
    	{
    		var foo = (int?) 1;
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 == foo);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var operandNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("=", operatorNode.Operator.ToString());
			Assert.AreEqual(1, operandNode.Value);
    	}

		[Test]
		public void Parse_WhenNullableIsComparedAgainstNullableVariableIsNull_ReturnsCorrectNodes()
		{
			var foo = (int?)null;
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 == foo);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var operandNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("=", operatorNode.Operator.ToString());
			Assert.AreEqual("null", operandNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableIsComparedAgainstValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 == 1);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var operandNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("=", operatorNode.Operator.ToString());
			Assert.AreEqual(1, operandNode.Value);
		}

		[Test]
		public void Parse_WhenNullableValueIsComparedAgainstValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.Value == 1);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var operandNode = (ValueNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("=", operatorNode.Operator.ToString());
			Assert.AreEqual(1, operandNode.Value);
		}

		[Test]
		public void Parse_WhenNullableIsNull_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 == null);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("is", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableIsNotNull_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1 != null);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstExplicitValueOfFalse_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue == false);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("is", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstExplicitValueOfTrue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue == true);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstImplicitValueOfTrue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt1.HasValue);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(3, listOfNodes.Count);

			var memberNode = (NullableMemberNode)listOfNodes[0];
			var operatorNode = (OperatorNode)listOfNodes[1];
			var nullNode = (NullNode)listOfNodes[2];

			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
			Assert.AreEqual("is not", operatorNode.Operator.ToString());
			Assert.AreEqual("null", nullNode.ToString());
		}

		[Test]
		public void Parse_WhenNullableHasValueIsComparedAgainstNegatedImplicitValueOfTrue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => !m.NullableInt1.HasValue);

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(4, listOfNodes.Count);

			var negationNode = (OperatorNode)listOfNodes[0];
			var memberNode = (NullableMemberNode)listOfNodes[1];
			var operatorNode = (OperatorNode)listOfNodes[2];
			var nullNode = (NullNode)listOfNodes[3];

			Assert.AreEqual("not", negationNode.Operator.ToString());
			Assert.AreEqual("NullableInt1", memberNode.Path);
			Assert.AreEqual(typeof(int), memberNode.DataType);
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