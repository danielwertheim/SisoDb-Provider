using System;
using System.Collections.Generic;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas;

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

		public IList<IParsedLambda> Includes { get; set; }

		public bool IsEmpty
		{
			get
			{
				return HasIncludes == false 
					&& HasPaging == false 
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

		public bool HasIncludes
		{
			get { return Includes != null && Includes.Count > 0; }
		}

		public Query(IStructureSchema structureSchema)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

			StructureSchema = structureSchema;

			Includes = new List<IParsedLambda>();
		}
	}
}