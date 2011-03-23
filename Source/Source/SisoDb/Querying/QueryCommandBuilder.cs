using System;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public class QueryCommandBuilder<T> : IQueryCommandBuilder<T> where T : class
    {
        public IWhereParser WhereParser { private get; set; }
        public ISortingParser SortingParser { private get; set; }
        public IIncludeParser IncludeParser { private get; set; }

        public IQueryCommand Command { get; private set; }

        public QueryCommandBuilder(IWhereParser whereParser, ISortingParser sortingParser, IIncludeParser includeParser)
        {
            WhereParser = whereParser.AssertNotNull("whereParser");
            SortingParser = sortingParser.AssertNotNull("sortingParser");
            IncludeParser = includeParser.AssertNotNull("includeParser");

            Command = new QueryCommand();
        }

        public IQueryCommandBuilder<T> Take(int numOfStructures)
        {
            Command.TakeNumOfStructures = numOfStructures.AssertGt(0, "numOfStructures");

            return this;
        }

        public IQueryCommandBuilder<T> Page(int pageIndex, int pageSize)
        {
            Command.Paging = new Paging(
                pageIndex.AssertGte(0, "pageIndex"), 
                pageSize.AssertGt(0, "pageSize"));

            return this;
        }

        public IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            predicate.AssertNotNull("predicate");

            if (Command.Where != null)
                throw new SisoDbException(ExceptionMessages.QueryCommandBuilder_WhereAllreadyInitialized);

            Command.Where = WhereParser.Parse(predicate);

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