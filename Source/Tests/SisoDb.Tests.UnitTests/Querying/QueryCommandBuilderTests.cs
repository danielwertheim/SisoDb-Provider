using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Querying
{
    [TestFixture]
    public class QueryCommandBuilderTests : UnitTestBase
    {
        [Test]
        public void Where_WhenExpressionAllreadyBeenAssigned_ThrowsSisoDbException()
        {
            var whereParserFake = new Mock<IWhereParser>();
            whereParserFake
                .Setup(f => f.Parse(It.IsAny<Expression<Func<MyClass, bool>>>()))
                .Returns(new Mock<IParsedLambda>().Object);
            var builder = CreateBuilderWithFakes();
            builder.WhereParser = whereParserFake.Object;

            var expression = Reflect<MyClass>.BoolExpressionFrom(m => m.Int1 == 1);
            builder.Where(expression);

            Assert.Throws<SisoDbException>(() => builder.Where(expression));
        }

        [Test]
        public void SortBy_WhenExpressionAllreadyBeenAssigned_ThrowsSisoDbException()
        {
            var sortingParserFake = new Mock<ISortingParser>();
            sortingParserFake
                .Setup(f => f.Parse(It.IsAny<Expression<Func<MyClass, object>>[]>()))
                .Returns(new Mock<IParsedLambda>().Object);
            var builder = CreateBuilderWithFakes();
            builder.SortingParser = sortingParserFake.Object;

            var expression = Reflect<MyClass>.ExpressionFrom<object>(m => m.Int1);
            builder.SortBy(expression);

            Assert.Throws<SisoDbException>(() => builder.SortBy(expression));
        }

        [Test]
        public void Take_WhenZeroNumOfStructures_ThrowsArgumentOutOfRangeException()
        {
            var builder = CreateBuilderWithFakes();

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Take(0));
        }

        [Test]
        public void Take_WhenPositiveValueIsPassedForNumOfStructures_ValueIsReflectedOnCommand()
        {
            var builder = CreateBuilderWithFakes();

            builder.Take(11);

            Assert.AreEqual(11, builder.Command.TakeNumOfStructures);
        }

        [Test]
        public void Paging_WhenPagingIsSet_PagingValuesAreReflectedInCommand()
        {
            var builder = CreateBuilderWithFakes();

            builder.Page(0, 10);

            Assert.AreEqual(new Paging(0, 10), builder.Command.Paging);
        }

        private QueryCommandBuilder<MyClass> CreateBuilderWithFakes()
        {
            var whereParserFake = new Mock<IWhereParser>();
            var sortingParserFake = new Mock<ISortingParser>();
            var includesParserFake = new Mock<IIncludeParser>();

            return new QueryCommandBuilder<MyClass>(whereParserFake.Object, sortingParserFake.Object, includesParserFake.Object);
        }

        private class MyClass
        {
            public int Int1 { get; set; }
        }
    }
}