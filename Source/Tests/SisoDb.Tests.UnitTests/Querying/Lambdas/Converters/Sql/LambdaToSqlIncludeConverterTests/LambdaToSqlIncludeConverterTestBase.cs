using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Parsers;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlIncludeConverterTests
{
    [TestFixture]
    public abstract class LambdaToSqlIncludeConverterTestBase : UnitTestBase
    {
        private readonly IIncludeParser _includeParser;

        protected LambdaToSqlIncludeConverterTestBase()
        {
            _includeParser = new IncludeParser();
        }

        internal IParsedLambda CreateParsedLambda<TInclude>(params LambdaExpression[] expressions) where TInclude : class
        {
            return _includeParser.Parse(StructureTypeNameFor<TInclude>.Name,expressions);
        }

        protected class Master
        {
            public Guid StructureId { get; set; }

            public Guid ChildOneId { get; set; }

            public Guid ChildTwoId { get; set; }

            public Nested NestedItem { get; set; }
        }

        protected class Nested
        {
            public Guid UnknownChildId { get; set; }
        }

        protected class ChildTypeA
        {
            public Guid StructureId { get; set; }
        }

        protected class ChildTypeB
        {
            public Guid StructureId { get; set; }
        }
    }
}