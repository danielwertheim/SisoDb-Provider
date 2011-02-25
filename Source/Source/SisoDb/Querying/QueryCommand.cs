using System;
using System.Collections.Generic;
using SisoDb.Lambdas;

namespace SisoDb.Querying
{
    [Serializable]
    public class QueryCommand : IQueryCommand
    {
        public IParsedLambda Selector { get; set; }

        public IParsedLambda Sortings { get; set; }

        public IList<IParsedLambda> Includes { get; private set; }

        public bool HasSelector
        {
            get { return Selector != null; }
        }

        public bool HasSortings
        {
            get { return Sortings != null && Sortings.Nodes.Count > 0; }
        }

        public bool HasIncludes
        {
            get { return Includes != null && Includes.Count > 0; }
        }

        public QueryCommand(IEnumerable<IParsedLambda> includes = null)
        {
            Includes = includes == null ? new List<IParsedLambda>() : new List<IParsedLambda>(includes);
        }
    }
}