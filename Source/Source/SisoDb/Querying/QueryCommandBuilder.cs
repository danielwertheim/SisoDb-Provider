using System;
using System.Linq.Expressions;
using SisoDb.Lambdas.Parsers;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public class QueryCommandBuilder<T> : IQueryCommandBuilder<T> where T : class
    {
        public ISelectorParser SelectorParser { private get; set; }
        public ISortingParser SortingParser { private get; set; }
        public IIncludeParser IncludeParser { private get; set; }

        public IQueryCommand Command { get; private set; }

        public QueryCommandBuilder(ISelectorParser selectorParser, ISortingParser sortingParser, IIncludeParser includeParser)
        {
            SelectorParser = selectorParser.AssertNotNull("selectorParser");
            SortingParser = sortingParser.AssertNotNull("sortingParser");
            IncludeParser = includeParser.AssertNotNull("includeParser");

            Command = new QueryCommand();
        }

        public IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> selector)
        {
            selector.AssertNotNull("selector");

            if (Command.Selector != null)
                throw new SisoDbException(ExceptionMessages.QueryCommandBuilder_WhereAllreadyInitialized);

            Command.Selector = SelectorParser.Parse(selector);

            return this;
        }

        public IQueryCommandBuilder<T> SortBy(params Expression<Func<T, object>>[] sortings)
        {
            sortings.AssertHasItems("sortings");

            if (Command.Sortings != null)
                throw new SisoDbException(ExceptionMessages.QueryCommandBuilder_SortingsAllreadyInitialized);

            Command.Sortings = SortingParser.Parse(sortings);

            return this;
        }

        public IQueryCommandBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] includes) where TInclude : class
        {
            includes.AssertHasItems("includes");

            Command.Includes.Add(IncludeParser.Parse<TInclude>(includes));

            return this;
        }
    }
}