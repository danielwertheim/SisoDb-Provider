using System.Collections.Generic;
using EnsureThat;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Converters.Sql
{
    public class LambdaToSqlSortingConverter : ILambdaToSqlSortingConverter
    {
        public IList<SqlSorting> Convert(IStructureSchema structureSchema, IParsedLambda lambda)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            
            var sortings = new List<SqlSorting>();

            if (lambda == null || lambda.Nodes.Count == 0)
            {
                sortings.Add(CreateDefaultStructureIdSorting(sortings.Count, SortDirections.Asc));

                return sortings;
            }

            foreach (var node in lambda.Nodes)
            {
                if(!(node is SortingNode))
                    throw new SisoDbException(ExceptionMessages.ParsedSortingLambdaSqlProcessor_NotSupportedNodeType.Inject(node.GetType().Name));

                var sortingNode = (SortingNode)node;

                var isStructureIdMember = (sortingNode.MemberPath == StructureStorageSchema.Fields.Id.Name);
                if(isStructureIdMember)
                {
                    sortings.Add(CreateDefaultStructureIdSorting(sortings.Count, sortingNode.Direction));
                    continue;
                }

                var valueField = IndexStorageSchema.GetValueSchemaFieldForType(sortingNode.MemberType);
                sortings.Add(new SqlSorting(
                    sortingNode.MemberPath,
                    "si.[{0}]".Inject(valueField.Name),
                    "[{0}]".Inject(valueField.Name),
                    "sort{0}".Inject(sortings.Count),
                    sortingNode.Direction.ToString()));
            }

            return sortings;
        }

        private static SqlSorting CreateDefaultStructureIdSorting(int sortingsCount, SortDirections sortDirection)
        {
            return new SqlSorting(
                "StructureStorageSchema.Fields.Id.Name",
                "s.[{0}]".Inject(StructureStorageSchema.Fields.Id.Name),
                "[{0}]".Inject(StructureStorageSchema.Fields.Id.Name),
                "sort{0}".Inject(sortingsCount),
                sortDirection.ToString());
        }
    }
}