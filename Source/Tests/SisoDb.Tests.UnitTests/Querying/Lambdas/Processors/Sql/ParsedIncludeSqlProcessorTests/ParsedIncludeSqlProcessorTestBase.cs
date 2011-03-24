using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Parsers;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Processors.Sql.ParsedIncludeSqlProcessorTests
{
    [TestFixture]
    public abstract class ParsedIncludeSqlProcessorTestBase : UnitTestBase
    {
        private readonly IIncludeParser _includeParser;

        protected ParsedIncludeSqlProcessorTestBase()
        {
            _includeParser = new IncludeParser();
        }

        internal IParsedLambda CreateParsedLambda<TInclude>(params LambdaExpression[] expressions) where TInclude : class
        {
            return _includeParser.Parse<TInclude>(expressions);
        }

        protected class Master
        {
            public Guid SisoId { get; set; }

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
            public Guid SisoId { get; set; }
        }

        protected class ChildTypeB
        {
            public Guid SisoId { get; set; }
        }
    }
}