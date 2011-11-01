using System;
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
        private class Session
        {
            public readonly List<string> SortingParts = new List<string>();
            
            public string GetSortingPartsString()
            {
                return string.Join(", ", SortingParts);
            }
        }

        public SqlSorting Convert(IStructureSchema structureSchema, IParsedLambda lambda)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(lambda, "lambda").IsNotNull();

            var session = new Session();

            foreach (var node in lambda.Nodes)
            {
                if (node is SortingNode)
                {
                    Add(session, (SortingNode)node);
                    continue;
                }

                throw new NotSupportedException(
                    ExceptionMessages.ParsedSortingLambdaSqlProcessor_NotSupportedNodeType.Inject(node.GetType().Name));
            }

            if(session.SortingParts.Count == 0)
                return SqlSorting.Empty();

            return new SqlSorting(session.GetSortingPartsString());
        }

        private static void Add(Session session, SortingNode sortingNode)
        {
            if(sortingNode.MemberPath == StructureStorageSchema.Fields.Id.Name)
            {
                session.SortingParts.Add(string.Format("s.[{0}] {1}", sortingNode.MemberPath, sortingNode.Direction));
                return;
            }

            session.SortingParts.Add(string.Format(
                "min(si.[{0}]) {1}", IndexStorageSchema.GetValueSchemaFieldForType(sortingNode.MemberType).Name, sortingNode.Direction));
        }
    }
}