using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Core;

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
            Assert.AreEqual("MyString", listOfNodes[2].CastAs<StringStartsWithMemberNode>().Path);
            Assert.AreEqual(typeof(string), listOfNodes[2].CastAs<StringStartsWithMemberNode>().DataType);
            Assert.AreEqual(Operator.Types.Or, listOfNodes[3].CastAs<OperatorNode>().Operator.OperatorType);
            Assert.AreEqual("MyInt", listOfNodes[4].CastAs<MemberNode>().Path);
            Assert.AreEqual(typeof(int), listOfNodes[4].CastAs<MemberNode>().DataType);
            Assert.AreEqual(Operator.Types.Equal, listOfNodes[5].CastAs<OperatorNode>().Operator.OperatorType);
            Assert.AreEqual(42, listOfNodes[6].CastAs<ValueNode>().Value);
            Assert.AreEqual(typeof(EndGroupNode), listOfNodes[7].GetType());
            Assert.AreEqual(Operator.Types.Or, listOfNodes[8].CastAs<OperatorNode>().Operator.OperatorType);
            Assert.AreEqual("MyListOfStrings", listOfNodes[9].CastAs<MemberNode>().Path);
            Assert.AreEqual(typeof(string), listOfNodes[9].CastAs<MemberNode>().DataType);
            Assert.AreEqual(Operator.Types.Equal, listOfNodes[10].CastAs<OperatorNode>().Operator.OperatorType);
            Assert.AreEqual("Bar", listOfNodes[11].CastAs<ValueNode>().Value);
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