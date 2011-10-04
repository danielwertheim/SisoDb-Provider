using System;
using System.Linq.Expressions;
using EnsureThat;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public class GetCommandBuilder<T> : IGetCommandBuilder<T> where T : class
    {
        public ISortingParser SortingParser { protected get; set; }
        public IIncludeParser IncludeParser { protected get; set; }

        public IGetCommand Command { get; private set; }

        public GetCommandBuilder(ISortingParser sortingParser, IIncludeParser includeParser)
        {
            Ensure.That(() => sortingParser).IsNotNull();
            Ensure.That(() => includeParser).IsNotNull();

            SortingParser = sortingParser;
            IncludeParser = includeParser;

            Command = new GetCommand();
        }

        public IGetCommandBuilder<T> SortBy(params Expression<Func<T, object>>[] sortings)
        {
            Ensure.That(() => sortings).HasItems();

            if (Command.Sortings != null)
                throw new SisoDbException(ExceptionMessages.GetCommand_SortingsAllreadyInitialized);

            Command.Sortings = SortingParser.Parse(sortings);

            return this;
        }

        public IGetCommandBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] includes) where TInclude : class
        {
            Ensure.That(() => includes).HasItems();

            Command.Includes.Add(IncludeParser.Parse(StructureTypeNameFor<TInclude>.Name, includes));

            return this;
        }
    }
}