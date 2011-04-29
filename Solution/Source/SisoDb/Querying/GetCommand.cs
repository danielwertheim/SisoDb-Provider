using System;
using System.Collections.Generic;
using SisoDb.Querying.Lambdas;

namespace SisoDb.Querying
{
    [Serializable]
    public class GetCommand : IGetCommand
    {
        public IParsedLambda Sortings { get; set; }

        public IList<IParsedLambda> Includes { get; private set; }

        public bool HasSortings
        {
            get { return Sortings != null && Sortings.Nodes.Count > 0; }
        }

        public bool HasIncludes
        {
            get { return Includes != null && Includes.Count > 0; }
        }

        public GetCommand()
        {
            Includes = new List<IParsedLambda>();
        }
    }
}