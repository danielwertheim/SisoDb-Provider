using System;
using System.Linq.Expressions;
using EnsureThat;
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
            Ensure.That(() => whereParser).IsNotNull();
            Ensure.That(() => sortingParser).IsNotNull();
            Ensure.That(() => includeParser).IsNotNull();

            WhereParser = whereParser;
            SortingParser = sortingParser;
            IncludeParser = includeParser;

            Command = new QueryCommand();
        }

        public IQueryCommandBuilder<T> Take(int numOfStructures)
        {
            Ensure.That(() => numOfStructures).IsGte(0);

            Command.TakeNumOfStructures = numOfStructures;

            return this;
        }

        public IQueryCommandBuilder<T> Page(int pageIndex, int pageSize)
        {
            Ensure.That(() => pageIndex).IsGte(0);
            Ensure.That(() => pageSize).IsGt(0);

            Command.Paging = new Paging(pageIndex, pageSize);

            return this;
        }

        public IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            Ensure.That(() => predicate).IsNotNull();

            if (Command.Where != null)
                throw new SisoDbException(ExceptionMessages.QueryCommandBuilder_WhereAllreadyInitialized);

            Command.Where = WhereParser.Parse(predicate);

            return this;
        }

        public IQueryCommandBuilder<T> SortBy(params Expression<Func<T, object>>[] sortings)
        {
            Ensure.That(() => sortings).HasItems();

            if (Command.Sortings != null)
                throw new SisoDbException(ExceptionMessages.QueryCommandBuilder_SortingsAllreadyInitialized);

            Command.Sortings = SortingParser.Parse(sortings);

            return this;
        }

        public IQueryCommandBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] includes) where TInclude : class
        {
            Ensure.That(() => includes).HasItems();

            Command.Includes.Add(
                IncludeParser.Parse(StructureTypeNameFor<TInclude>.Name, includes));

            return this;
        }
    }
}