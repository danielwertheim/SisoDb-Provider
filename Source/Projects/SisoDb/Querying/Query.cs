using System;
using SisoDb.EnsureThat;
using SisoDb.Querying.Lambdas;
using SisoDb.Structures.Schemas;

namespace SisoDb.Querying
{
    [Serializable]
	public class Query : IQuery
	{
		public IStructureSchema StructureSchema { get; private set; }
        public int? TakeNumOfStructures { get; set; }
        public Paging Paging { get; set; }
        public IParsedLambda Where { get; set; }
        public IParsedLambda Sortings { get; set; }
        public bool IsCacheable { get; set; }

		public bool IsEmpty
		{
			get
			{
				return HasPaging == false 
					&& HasSortings == false 
					&& HasTakeNumOfStructures == false
					&& HasWhere == false;
			}
		}

		public bool HasTakeNumOfStructures
		{
			get { return TakeNumOfStructures.HasValue && TakeNumOfStructures > 0; }
		}

		public bool HasPaging
		{
			get { return Paging != null; }
		}

		public bool HasWhere
		{
			get { return Where != null && Where.Nodes.Length > 0; }
		}

		public bool HasSortings
		{
			get { return Sortings != null && Sortings.Nodes.Length > 0; }
		}

		public Query(IStructureSchema structureSchema)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

			StructureSchema = structureSchema;
		    IsCacheable = false;
		}
	}
}