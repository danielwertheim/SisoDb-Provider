using System.Collections.Generic;
using SisoDb.Lambdas;

namespace SisoDb.Querying
{
    public interface IQueryCommand
    {
        int TakeNumOfStructures { get; set; }

        Paging Paging { get; set; }

        IParsedLambda Where { get; set; }

        IParsedLambda Sortings { get; set; }

        IList<IParsedLambda> Includes { get; }

        bool HasTakeNumOfStructures { get; }

        bool HasPaging { get; }

        bool HasWhere { get; }

        bool HasSortings { get; }

        bool HasIncludes { get; }
    }
}