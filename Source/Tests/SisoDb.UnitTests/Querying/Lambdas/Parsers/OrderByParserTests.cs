using System;
using NUnit.Framework;
using SisoDb.NCore;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;

namespace SisoDb.UnitTests.Querying.Lambdas.Parsers
{
	[TestFixture]
	public class OrderByParserTests : UnitTestBase
	{
        private readonly DataTypeConverter _dataTypeConverter = new DataTypeConverter();

        private OrderByParser CreateParser()
        {
            return new OrderByParser(_dataTypeConverter);
        }

		[Test]
		public void Parse_WhenNonMemberExpression_ThrowsException()
		{
			var parser = CreateParser();
			var nonMemberExpression = new OrderByAscExpression(Reflect<MyClass>.BoolExpressionFrom(m => m.Int1 == 32));

			var ex = Assert.Throws<SisoDbException>(() => parser.Parse(new[] { nonMemberExpression }));

            Assert.AreEqual(ExceptionMessages.OrderByExpressionDoesNotTargetMember.Inject("m => (m.Int1 == 32)"), ex.Message);
		}

		[Test]
		public void Parse_WhenNullExpressions_ThrowsException()
		{
			Assert.Throws<ArgumentException>(
				() => CreateParser().Parse(null));
		}

		[Test]
		public void Parse_WhenEmptyExpressions_ThrowsException()
		{
			Assert.Throws<ArgumentException>(
				() => CreateParser().Parse(new OrderByExpression[] { }));
		}

		[Test]
		public void Parse_WhenOneExpressionIsNullExpression_DoesNotThrowException()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));

			Assert.DoesNotThrow(
				() => CreateParser().Parse(new OrderByExpression[] { lambda, null }));
		}

		[Test]
		public void Parse_WhenNestedMemberIsMethod_ThrowsException()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.Child.DoSomething()));

			var ex = Assert.Throws<SisoDbException>(
				() => CreateParser().Parse(new[] { lambda }));

			Assert.AreEqual(ExceptionMessages.OrderByParser_UnsupportedMethodForSortingDirection, ex.Message);
		}

		[Test]
		public void Parse_WhenOnePropertyLambdaIsPassed_ExactlyOneSortingNodeIsReturned()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			Assert.AreEqual(1, parsedLambda.Nodes.Length);
			Assert.IsInstanceOf(typeof(SortingNode), parsedLambda.Nodes[0]);
		}

		[Test]
		public void Parse_WhenTwoPropertyLambdasArePassed_ExactlyTwoSortingNodeIsReturned()
		{
			var lambda1 = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));
			var lambda2 = new OrderByDescExpression(Reflect<MyClass>.LambdaFrom(m => m.Int1));

			var parsedLambda = CreateParser().Parse(new OrderByExpression[] { lambda1, lambda2 });

			Assert.AreEqual(2, parsedLambda.Nodes.Length);
			Assert.IsInstanceOf(typeof(SortingNode), parsedLambda.Nodes[0]);
			Assert.IsInstanceOf(typeof(SortingNode), parsedLambda.Nodes[1]);
		}

		[Test]
		public void Parse_WhenFirstLevelLambda_MemberPathIsPropertyName()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			var node = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual("String1", node.MemberPath);
		}

		[Test]
		public void Parse_WhenNestedLambda_MemberPathReflectsHierarchy()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.Child.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			var node = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual("Child.String1", node.MemberPath);
		}

		[Test]
		public void Parse_WhenAscExpression_SortingDirectionBecomesAsc()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			var node = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual(SortDirections.Asc, node.Direction);
		}

		[Test]
		public void Parse_WhenAscExpressionOnNested_SortingDirectionBecomesAsc()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.Child.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			var node = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual(SortDirections.Asc, node.Direction);
		}

		[Test]
		public void Parse_WhenAscExpressionOnNested_MemberPathReflectsHierarchy()
		{
			var lambda = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.Child.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			var node = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual("Child.String1", node.MemberPath);
		}

		[Test]
		public void Parse_WhenDescExpression_SortingDirectionBecomesDesc()
		{
			var lambda = new OrderByDescExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));

			var parsedLambda = CreateParser().Parse(new[] { lambda });

			var node = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual(SortDirections.Desc, node.Direction);
		}

		[Test]
		public void Parse_WhenAscAndDescExpressionOnSameMember_SortingDirectionBecomesBothAscAndDescInCorrectOrder()
		{
			var lambda1 = new OrderByAscExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));
			var lambda2 = new OrderByDescExpression(Reflect<MyClass>.LambdaFrom(m => m.String1));

			var parsedLambda = CreateParser()
				.Parse(new OrderByExpression [] { lambda1, lambda2 });

			Assert.AreEqual(2, parsedLambda.Nodes.Length);

			var node1 = (SortingNode)parsedLambda.Nodes[0];
			Assert.AreEqual(SortDirections.Asc, node1.Direction);

			var node2 = (SortingNode)parsedLambda.Nodes[1];
			Assert.AreEqual(SortDirections.Desc, node2.Direction);
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