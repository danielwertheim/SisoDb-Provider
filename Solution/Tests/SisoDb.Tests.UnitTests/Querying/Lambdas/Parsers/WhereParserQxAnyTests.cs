using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserQxAnyTests : UnitTestBase
    {
        [Test]
        public void Parse_WhenQxAnyIsUsedWithLtOperator_ThrowsSisoDbException()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Value < 42));

            var parser = new WhereParser();
            var ex = Assert.Throws<SisoDbException>(() => parser.Parse(expression));

            Assert.AreEqual("The operator 'LessThan' is not supported for Qx-methods on enumerables.", ex.Message);
        }

        [Test]
        public void Parse_WhenQxAnyIsUsedWithLteOperator_ThrowsSisoDbException()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Value <= 42));

            var parser = new WhereParser();
            var ex = Assert.Throws<SisoDbException>(() => parser.Parse(expression));

            Assert.AreEqual("The operator 'LessThanOrEqual' is not supported for Qx-methods on enumerables.", ex.Message);
        }

        [Test]
        public void Parse_WhenQxAnyIsUsedWithGtOperator_ThrowsSisoDbException()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Value > 42));

            var parser = new WhereParser();
            var ex = Assert.Throws<SisoDbException>(() => parser.Parse(expression));
            
            Assert.AreEqual("The operator 'GreaterThan' is not supported for Qx-methods on enumerables.", ex.Message);
        }

        [Test]
        public void Parse_WhenQxAnyIsUsedWithGteOperator_ThrowsSisoDbException()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Value >= 42));

            var parser = new WhereParser();
            var ex = Assert.Throws<SisoDbException>(() => parser.Parse(expression));

            Assert.AreEqual("The operator 'GreaterThanOrEqual' is not supported for Qx-methods on enumerables.", ex.Message);
        }

        [Test]
        public void Parse_WhenQxAnyIsUsedWithEqOperator_ReturnsCorrectNodes()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Value == 42));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode) listOfNodes[2];
            Assert.AreEqual("ChildA.Items.Value", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("%<$42$>%", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxAnyIsUsedWithNotEqOperator_ReturnsCorrectNodes()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Value != 42));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode1 = (OperatorNode)listOfNodes[1];
            var operatorNode2 = (OperatorNode)listOfNodes[2];
            var operandNode = (ValueNode)listOfNodes[3];
            Assert.AreEqual("ChildA.Items.Value", memberNode.Path);
            Assert.AreEqual("not", operatorNode1.Operator.ToString());
            Assert.AreEqual("like", operatorNode2.Operator.ToString());
            Assert.AreEqual("%<$42$>%", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxAnyOnQxAnyIsUsedWithEqOperator_ReturnsCorrectNodes()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Values.QxAny(i2 => i2 == 42)));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var memberNode = (MemberNode)listOfNodes[0];
            var operatorNode = (OperatorNode)listOfNodes[1];
            var operandNode = (ValueNode)listOfNodes[2];
            Assert.AreEqual("ChildA.Items.Values", memberNode.Path);
            Assert.AreEqual("like", operatorNode.Operator.ToString());
            Assert.AreEqual("%<$42$>%", operandNode.Value);
        }

        [Test]
        public void Parse_WhenQxAnyOnQxAnyIsUsedWithOrEqOperator_ReturnsCorrectNodes()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Values.QxAny(i2 => i2 == 42 || i2 == 142)));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var startGroupNode = (StartGroupNode)listOfNodes[0];
            var memberNode1 = (MemberNode)listOfNodes[1];
            var operatorNode1 = (OperatorNode)listOfNodes[2];
            var operandNode1 = (ValueNode)listOfNodes[3];
            var groupOperatorNode = (OperatorNode)listOfNodes[4];
            var memberNode2 = (MemberNode)listOfNodes[5];
            var operatorNode2 = (OperatorNode)listOfNodes[6];
            var operandNode2 = (ValueNode)listOfNodes[7];
            var endGroupNode = (EndGroupNode)listOfNodes[8];
            Assert.AreEqual("ChildA.Items.Values", memberNode1.Path);
            Assert.AreEqual("like", operatorNode1.Operator.ToString());
            Assert.AreEqual("%<$42$>%", operandNode1.Value);
            Assert.AreEqual("or", groupOperatorNode.Operator.ToString());
            Assert.AreEqual("ChildA.Items.Values", memberNode2.Path);
            Assert.AreEqual("like", operatorNode2.Operator.ToString());
            Assert.AreEqual("%<$142$>%", operandNode2.Value);
        }

        [Test]
        public void Parse_WhenQxAnyOnQxAnyIsUsedWithAndNotEqOperator_ReturnsCorrectNodes()
        {
            var expression = Reflect<Root>.LambdaFrom(m => m.ChildA.Items.QxAny(i => i.Values.QxAny(i2 => i2 != 42 && i2 != 142)));

            var parser = new WhereParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            var startGroupNode = (StartGroupNode)listOfNodes[0];
            var memberNode1 = (MemberNode)listOfNodes[1];
            var isNotOperatorNode1 = (OperatorNode)listOfNodes[2];
            var operatorNode1 = (OperatorNode)listOfNodes[3];
            var operandNode1 = (ValueNode)listOfNodes[4];
            var groupOperatorNode = (OperatorNode)listOfNodes[5];
            var memberNode2 = (MemberNode)listOfNodes[6];
            var isNotOperatorNode2 = (OperatorNode)listOfNodes[7];
            var operatorNode2 = (OperatorNode)listOfNodes[8];
            var operandNode2 = (ValueNode)listOfNodes[9];
            var endGroupNode = (EndGroupNode)listOfNodes[10];
            Assert.AreEqual("ChildA.Items.Values", memberNode1.Path);
            Assert.AreEqual("not", isNotOperatorNode1.Operator.ToString());
            Assert.AreEqual("like", operatorNode1.Operator.ToString());
            Assert.AreEqual("%<$42$>%", operandNode1.Value);
            Assert.AreEqual("and", groupOperatorNode.Operator.ToString());
            Assert.AreEqual("ChildA.Items.Values", memberNode2.Path);
            Assert.AreEqual("not", isNotOperatorNode2.Operator.ToString());
            Assert.AreEqual("like", operatorNode2.Operator.ToString());
            Assert.AreEqual("%<$142$>%", operandNode2.Value);
        }

        public class Root
        {
            public FirstLevelChild ChildA { get; set; }

            public Root()
            {
                ChildA = new FirstLevelChild();
            }
        }

        public class FirstLevelChild
        {
            public string Name { get; set; }

            public List<Item> Items { get; set; }

            public FirstLevelChild()
            {
                Items = new List<Item>();
            }
        }

        public class Item
        {
            public int Value { get; set; }

            public int[] Values { get; set; }
        }       
    }
}