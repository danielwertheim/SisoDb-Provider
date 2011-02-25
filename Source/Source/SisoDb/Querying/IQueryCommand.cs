using System.Collections.Generic;
using SisoDb.Lambdas;

namespace SisoDb.Querying
{
    public interface IQueryCommand
    {
        IParsedLambda Selector { get; set; }

        IParsedLambda Sortings { get; set; }

        IList<IParsedLambda> Includes { get; }

        bool HasSelector { get; }

        bool HasSortings { get; }

        bool HasIncludes { get; }
    }
}