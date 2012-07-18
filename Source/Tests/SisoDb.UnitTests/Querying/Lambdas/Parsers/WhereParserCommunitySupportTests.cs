using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Querying.Lambdas.Parsers;

namespace SisoDb.UnitTests.Querying.Lambdas.Parsers
{
    [TestFixture]
    public class WhereParserCommunitySupportTests : UnitTestBase
    {
        private readonly DataTypeConverter _dataTypeConverter = new DataTypeConverter();

        private WhereParser CreateParser()
        {
            return new WhereParser(_dataTypeConverter);
        }

        [Test]
        public void Parse_When_StringQxStartsWith_or_IntEquals_or_ListOfStringsQxAny__ReturnsCorrectNodes()
        {
            var expression = Reflect<MyClass>.LambdaFrom(m => m.MyString.QxStartsWith("Foo") || m.MyInt == 42 || m.MyListOfStrings.QxAny(i => i == "Bar"));

            var parser = CreateParser();
            var parsedLambda = parser.Parse(expression);

            var listOfNodes = parsedLambda.Nodes.ToList();
            Assert.AreEqual(13, listOfNodes.Count);

            Assert.AreEqual(typeof(StartGroupNode), listOfNodes[0].GetType());
            Assert.AreEqual(typeof(StartGroupNode), listOfNodes[1].GetType());
            Assert.AreEqual("MyString", ((StringStartsWithMemberNode)listOfNodes[2]).Path);
            Assert.AreEqual(typeof(string), ((StringStartsWithMemberNode)listOfNodes[2]).DataType);
            Assert.AreEqual(Operator.Types.Or, ((OperatorNode)listOfNodes[3]).Operator.OperatorType);
            Assert.AreEqual("MyInt", ((MemberNode)listOfNodes[4]).Path);
            Assert.AreEqual(typeof(int), ((MemberNode)listOfNodes[4]).DataType);
            Assert.AreEqual(Operator.Types.Equal, ((OperatorNode)listOfNodes[5]).Operator.OperatorType);
            Assert.AreEqual(42, ((ValueNode)listOfNodes[6]).Value);
            Assert.AreEqual(typeof(EndGroupNode), listOfNodes[7].GetType());
            Assert.AreEqual(Operator.Types.Or, ((OperatorNode)listOfNodes[8]).Operator.OperatorType);
            Assert.AreEqual("MyListOfStrings", ((MemberNode)listOfNodes[9]).Path);
            Assert.AreEqual(typeof(string), ((MemberNode)listOfNodes[9]).DataType);
            Assert.AreEqual(Operator.Types.Equal, ((OperatorNode)listOfNodes[10]).Operator.OperatorType);
            Assert.AreEqual("Bar", ((ValueNode)listOfNodes[11]).Value);
            Assert.AreEqual(typeof(EndGroupNode), listOfNodes[12].GetType());
        }

        private class MyClass
        {
            public string MyString { get; set; }
            public int MyInt { get; set; }
            public List<string> MyListOfStrings { get; set; }
        }
    }
}