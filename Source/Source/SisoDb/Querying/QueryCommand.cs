using System;
using System.Collections.Generic;
using SisoDb.Querying.Lambdas;

namespace SisoDb.Querying
{
    [Serializable]
    public class QueryCommand : IQueryCommand
    {
        public int TakeNumOfStructures { get; set; }

        public Paging Paging { get; set; }

        public IParsedLambda Where { get; set; }

        public IParsedLambda Sortings { get; set; }

        public IList<IParsedLambda> Includes { get; set; }

        public bool HasTakeNumOfStructures
        {
            get { return TakeNumOfStructures > 0; }
        }

        public bool HasPaging
        {
            get { return Paging != null; }
        }

        public bool HasWhere
        {
            get { return Where != null && Where.Nodes.Count > 0; }
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