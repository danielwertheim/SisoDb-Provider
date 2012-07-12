using System.Linq;
using System.Linq.Expressions;
using NCore;
using NUnit.Framework;
using PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;

namespace SisoDb.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserStringQueryTests : UnitTestBase
    {
        private readonly DataTypeConverter _dataTypeConverter = new DataTypeConverter();

        private WhereParser CreateParser()
        {
            return new WhereParser(_dataTypeConverter);
        }

        [Test]
        public void Parse_WhenToLower_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.ToLower() == "foo");

            var parser = CreateParser();
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
        public void Parse_WhenToLowerOnInlineValue_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1 == "FOO".ToLower());

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("foo", operandNode.Value);
        }

        [Test]
        public void Parse_WhenToLowerOnInlineVariable_ReturnsCorrectNodes()
        {
            var param = "FOO";
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1 == param.ToLower());

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
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

            var parser = CreateParser();
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
        public void Parse_WhenToUpperOnInlineValue_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1 == "foo".ToUpper());

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("FOO", operandNode.Value);
        }

        [Test]
        public void Parse_WhenToUpperOnInlineVariable_ReturnsCorrectNodes()
        {
            var param = "foo";
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1 == param.ToUpper());

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("=", operatorNode.Operator.ToString());
            Assert.AreEqual("FOO", operandNode.Value);
        }

		[Test]
		public void Parse_WhenStartsWith_AgainstValueOfProperty_ReturnsCorrectNodes()
		{
			var q = new StartsWithQueryObject { Value = "Foo" };
			var expression = q.CreateExpression();

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringStartsWithMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo", memberNode.Value);
		}

        [Test]
        public void Parse_WhenStartsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.StartsWith("Foo"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringStartsWithMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo", memberNode.Value);
        }

        [Test]
        public void Parse_WhenEndsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.EndsWith("bar"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringEndsWithMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("bar", memberNode.Value);
        }

        [Test]
        public void Parse_WhenQxToLower_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxToLower() == "foo");

            var parser = CreateParser();
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

            var parser = CreateParser();
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

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringStartsWithMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo", memberNode.Value);
        }

        [Test]
        public void Parse_WhenQxEndsWith_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxEndsWith("bar"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringEndsWithMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("bar", memberNode.Value);
        }

        [Test]
        public void Parse_WhenQxLike_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxLike("Foo%bar"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (LikeMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo%bar", memberNode.Value);
        }

        [Test]
        public void Parse_WhenQxContains_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxContains("Foo"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringContainsMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo", memberNode.Value);
        }

        [Test]
        public void Parse_WhenContains_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.Contains("Foo"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringContainsMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo", memberNode.Value);
        }

        [Test]
        public void Parse_WhenContainsWithNull_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.Contains(null));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringContainsMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual(null, memberNode.Value);
        }

		[Test]
		public void Parse_WhenStartsWithOnToStringOfNullable_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.ToString().StartsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringStartsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenQxStartsWithOnToStringOfNullable_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.ToString().QxStartsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringStartsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenStartsWithOnToStringOfNullableValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.Value.ToString().StartsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringStartsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenQxStartsWithOnToStringOfNullableValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.Value.ToString().QxStartsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringStartsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenEndsWithOnToStringOfNullable_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.ToString().EndsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringEndsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
            Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenQxEndsWithOnToStringOfNullable_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.ToString().QxEndsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringEndsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenEndsWithOnToStringOfNullableValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.Value.ToString().EndsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringEndsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

		[Test]
		public void Parse_WhenQxEndsWithOnToStringOfNullableValue_ReturnsCorrectNodes()
		{
			var expression = Reflect<MyClass>.LambdaFrom(m => m.NullableInt.HasValue && m.NullableInt.Value.ToString().QxEndsWith("42"));

			var parser = CreateParser();
			var parsedLambda = parser.Parse(expression);

			var listOfNodes = parsedLambda.Nodes.ToList();
			Assert.AreEqual(5, listOfNodes.Count);

			var memberNode1 = (NullableMemberNode)listOfNodes[0];
			var isNotNode1 = (OperatorNode)listOfNodes[1];
			var nullNode1 = (NullNode)listOfNodes[2];
			var andNode = (OperatorNode)listOfNodes[3];
			var memberNode2 = (StringEndsWithMemberNode)listOfNodes[4];

			Assert.AreEqual("NullableInt", memberNode1.Path);
			Assert.AreEqual(typeof(int), memberNode1.DataType);
			Assert.AreEqual("is not", isNotNode1.Operator.ToString());
			Assert.AreEqual("null", nullNode1.ToString());
			Assert.AreEqual("and", andNode.ToString());
			Assert.AreEqual("NullableInt", memberNode2.Path);
			Assert.AreEqual(typeof(int), memberNode2.DataType);
			Assert.AreEqual("42", memberNode2.Value);
		}

        [Test]
        public void Parse_WhenQxIsExactly_for_simple_string_member_ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.String1.QxIsExactly("Foo"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(1, listOfNodes.Count);

            var memberNode = (StringExactMemberNode)listOfNodes[0];
            Assert.AreEqual("String1", memberNode.Path);
            Assert.AreEqual("Foo", memberNode.Value);
        }

        [Test]
        public void Parse_WhenQxIsExactly_for_text_member_ThrowsNotSupportedException()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.SomeText.QxIsExactly("Foo"));
            var parser = CreateParser();
            
            var ex = Assert.Throws<SisoDbNotSupportedException>(() => parser.Parse(expression));
            
            Assert.AreEqual(ExceptionMessages.QxIsExactly_NotSupportedForTexts.Inject("SomeText"), ex.Message);
        }

        private class StartsWithQueryObject
        {
            public string Value { get; set; }

            public LambdaExpression CreateExpression()
            {
                return Reflect<MyClass>.LambdaFrom(m => m.String1.StartsWith(Value));
            }
        }

        private class MyClass
        {
            public string String1 { get; set; }

            public string SomeText { get; set; }

			public int? NullableInt { get; set; }
        }
    }
}