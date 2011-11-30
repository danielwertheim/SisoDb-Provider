using System.Collections.Generic;
using PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas;

namespace SisoDb.Querying
{
    public interface IQueryCommand
    {
        IStructureSchema StructureSchema { get; }
        int? TakeNumOfStructures { get; set; }
        Paging Paging { get; set; }
        IParsedLambda Where { get; set; }
        IParsedLambda Sortings { get; set; }
        IList<IParsedLambda> Includes { get; set; }
        bool HasTakeNumOfStructures { get; }
        bool HasPaging { get; }
        bool HasWhere { get; }
        bool HasSortings { get; }
        bool HasIncludes { get; }
    }
}