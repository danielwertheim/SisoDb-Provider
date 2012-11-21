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
        bool IsCacheable { get; set; }

		bool IsEmpty { get; }
        bool HasTakeNumOfStructures { get; }
		bool HasPaging { get; }
		bool HasWhere { get; }
		bool HasSortings { get; }
	}
}