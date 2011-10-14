using System;
using System.Collections.Generic;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas;

namespace SisoDb.Querying
{
    [Serializable]
    public class QueryCommand : IQueryCommand
    {
        public IStructureSchema StructureSchema { get; private set; }

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

        public QueryCommand(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            StructureSchema = structureSchema;

            Includes = new List<IParsedLambda>();
        }
    }
}