using System.Collections.Generic;
using SisoDb.Lambdas;

namespace SisoDb.Querying
{
    public interface IGetCommand
    {
        IParsedLambda Sortings { get; set; }

        IList<IParsedLambda> Includes { get; }

        bool HasSortings { get; }

        bool HasIncludes { get; }
    }
}