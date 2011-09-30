using System;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.Querying
{
    public class GetCommandBuilder<T> : IGetCommandBuilder<T> where T : class
    {
        public ISortingParser SortingParser { protected get; set; }
        public IIncludeParser IncludeParser { protected get; set; }

        public IGetCommand Command { get; private set; }

        public GetCommandBuilder(ISortingParser sortingParser, IIncludeParser includeParser)
        {
            SortingParser = sortingParser.AssertNotNull("sortingParser");
            IncludeParser = includeParser.AssertNotNull("includeParser");

            Command = new GetCommand();
        }

        public IGetCommandBuilder<T> SortBy(params Expression<Func<T, object>>[] sortings)
        {
            sortings.AssertHasItems("sortings");

            if (Command.Sortings != null)
                throw new SisoDbException(ExceptionMessages.GetCommand_SortingsAllreadyInitialized);

            Command.Sortings = SortingParser.Parse(sortings);

            return this;
        }

        public IGetCommandBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] includes) where TInclude : class
        {
            includes.AssertHasItems("includes");

            Command.Includes.Add(
                IncludeParser.Parse(
                    StructureTypeNameFor<TInclude>.Name,
                    includes));

            return this;
        }
    }
}