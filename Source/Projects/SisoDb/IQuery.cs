using System.Collections.Generic;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
	public interface IQuery
	{
		IStructureSchema StructureSchema { get; }
		int? TakeNumOfStructures { get; set; }
		Paging Paging { get; set; }
		IParsedLambda Where { get; set; }
		IParsedLambda Sortings { get; set; }
        //TODO: Rem for v16.0.0 final
        //IList<IParsedLambda> Includes { get; set; }

		bool IsEmpty { get; }
		bool HasTakeNumOfStructures { get; }
		bool HasPaging { get; }
		bool HasWhere { get; }
		bool HasSortings { get; }
        //TODO: Rem for v16.0.0 final
        //bool HasIncludes { get; }
	}
}