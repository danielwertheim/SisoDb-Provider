using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Lambdas.ParsedLambdaSqlProcessorTests
{
    [TestFixture]
    public abstract class ParsedLambdaSqlProcessorTestBase : UnitTestBase
    {
        private readonly ILambdaParser _lambdaParser;

        protected ParsedLambdaSqlProcessorTestBase()
        {
            _lambdaParser = new LambdaParser();
        }

        internal IParsedLambda CreateParsedLambda<T>(Expression<Func<T, bool>> e)
        {
            return _lambdaParser.Parse(e);
        }

        protected void AssertQueryParameters(IEnumerable<IQueryParameter> expected, IEnumerable<IQueryParameter> actual)
        {
            CustomAssert.AreEqual(expected, actual, (x, y) => x.Name.Equals(y.Name) && Equals(x.Value, y.Value));
        }

        protected class MyItem
        {
            public int Int1 { get; set; }
            public int Int2 { get; set; }
            public decimal Decimal1 { get; set; }
            public bool Bool1 { get; set; }
            public DateTime DateTime1 { get; set; }
            public Guid Guid1 { get; set; }
            public string String1 { get; set; }
            public string[] Strings { get; set; }
            public IList<int> ListOfIntegers { get; set; }
            public decimal[] Decimals { get; set; }

            public MyNestedItem NestedItem { get; set; }

            public IList<ChildItem> ChildItems { get; set; }
        }

        protected class ChildItem
        {
            public int Int1 { get; set; }

            public string String1 { get; set; }

            public MyNestedItem NestedItem { get; set; }

            public IList<GrandChild> GrandChildItems { get; set; }

            public int[] Integers { get; set; }
        }

        public class GrandChild
        {
            public int Int1 { get; set; }
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