using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Parsers;

namespace SisoDb.Tests.UnitTests.Lambdas.Processors.ParsedSortingSqlProcessorTests
{
    [TestFixture]
    public abstract class ParsedSortingSqlProcessorTestBase : UnitTestBase
    {
        private readonly ISortingParser _sortingParser;

        protected ParsedSortingSqlProcessorTestBase()
        {
            _sortingParser = new SortingParser();
        }

        internal IParsedLambda CreateParsedLambda<T>(params Expression<Func<T, object>>[] es) where T : class 
        {
            return _sortingParser.Parse(es);
        }

        protected class MyItem
        {
            public int Id { get; set; }
            public int Int1 { get; set; }
            public decimal Decimal1 { get; set; }
            public bool Bool1 { get; set; }
            public DateTime DateTime1 { get; set; }
            public Guid Guid1 { get; set; }
            public string String1 { get; set; }

            public MyNestedItem NestedItem { get; set; }
        }

        protected class MyNestedItem
        {
            public int Int1 { get; set; }

            public MySuperNestedItem SuperNestedItem { get; set; }
        }

        protected class MySuperNestedItem
        {
            public int Int1 { get; set; }
        }
    }
}