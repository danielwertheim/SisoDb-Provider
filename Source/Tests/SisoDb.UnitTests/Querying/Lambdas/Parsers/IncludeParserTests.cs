using System;
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
    public class IncludeParserTests : UnitTestBase
    {
        private readonly DataTypeConverter _dataTypeConverter = new DataTypeConverter();

        private IncludeParser CreateParser()
        {
            return new IncludeParser(_dataTypeConverter);
        }

        [Test]
        public void Parse_WhenNonMemberExpression_ThrowsSisoDbException()
        {
            var parser = CreateParser();
            var nonMemberExpression = Reflect<Master>.BoolExpressionFrom(m => m.Int1 == 32);

            var ex = Assert.Throws<SisoDbException>(() => parser.Parse(StructureTypeNameFor<Child>.Name, new[] { nonMemberExpression }));

            Assert.AreEqual(ExceptionMessages.IncludeExpressionDoesNotTargetMember.Inject("m => (m.Int1 == 32)"), ex.Message);
        }

        [Test]
        public void Parse_WhenNullExpressions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => CreateParser().Parse(StructureTypeNameFor<ChildWithGuidId>.Name, null));
        }

        [Test]
        public void Parse_WhenEmptyExpressions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => CreateParser().Parse(StructureTypeNameFor<ChildWithGuidId>.Name, new LambdaExpression[0]));
        }

        [Test]
        public void Parse_WhenMemberExpressionIsPassed_GeneratesExactlyOneIncludeNode()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            var parser = CreateParser();
            var parsedLambda = parser.Parse(StructureTypeNameFor<ChildWithGuidId>.Name, new[] { lambda });

            Assert.AreEqual(1, parsedLambda.Nodes.Length);
            Assert.IsNotNull(parsedLambda.Nodes[0] as IncludeNode);
        }

        [Test]
        public void Parse_WhenMemberExpressionIsPassed_HasCorrectParentMemberNode()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            var parser = CreateParser();
            var parsedLambda = parser.Parse(StructureTypeNameFor<ChildWithGuidId>.Name, new[] { lambda });
            var includeNode = (IncludeNode)parsedLambda.Nodes[0];

            Assert.AreEqual("ChildId", includeNode.IdReferencePath);
        }

        [Test]
        public void Parse_WhenNestedMemberExpressionIsPassed_HasCorrectParentMemberNode()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.Child.GrandChild.StructureId);

            var parser = CreateParser();
            var parsedLambda = parser.Parse(StructureTypeNameFor<ChildWithGuidId>.Name, new[] { lambda });
            var includeNode = (IncludeNode)parsedLambda.Nodes[0];

            Assert.AreEqual("Child.GrandChild.StructureId", includeNode.IdReferencePath);
        }

        [Test]
        public void Parse_WhenMemberExpressionIsPassed_HasCorrectChildStructureName()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildId);

            var parser = CreateParser();
            var parsedLambda = parser.Parse(StructureTypeNameFor<ChildWithGuidId>.Name, new[] { lambda });
            var includeNode = (IncludeNode)parsedLambda.Nodes[0];

            Assert.AreEqual("ChildWithGuidId", includeNode.ReferencedStructureName);
        }

    	private static class StructureTypeNameFor<T> where T : class
		{
			public static readonly string Name = typeof(T).Name;
		}

        private class Master
        {
            public Guid StructureId { get; set; }

            public int Int1 { get; set; }

            public Guid ChildId { get; set; }

            public Child Child { get; set; }
        }

        private class ChildWithGuidId
        {
            public Guid StructureId { get; set; }
        }

        private class Child
        {
            public GrandChild GrandChild { get; set; }
        }

        private class GrandChild
        {
            public Guid StructureId { get; set; }
        }
    }
}