using System;
using System.Collections.Generic;
using EnsureThat;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb.Querying.Lambdas.Converters.Sql
{
    public class LambdaToSqlSortingConverter : ILambdaToSqlSortingConverter
    {
        private class Session
        {
            public readonly IStructureSchema StructureSchema;
            public readonly List<string> Joins = new List<string>();
            public readonly List<string> SortingParts = new List<string>();

            public Session(IStructureSchema structureSchema)
            {
                StructureSchema = structureSchema;
            }

            public string GetJoinsString()
            {
                return string.Join(" ", Joins);
            }

            public string GetSortingPartsString()
            {
                return string.Join(", ", SortingParts);
            }
        }

        public SqlSorting Convert(IStructureSchema structureSchema, IParsedLambda lambda)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(lambda, "lambda").IsNotNull();

            var session = new Session(structureSchema);

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

            if(session.Joins.Count == 0 && session.SortingParts.Count == 0)
                return SqlSorting.Empty();

            return session.Joins.Count == 0 
                ? new SqlSorting(session.GetSortingPartsString())
                : new SqlSorting(session.GetSortingPartsString(), session.GetJoinsString());
        }

        private static void Add(Session session, SortingNode sortingNode)
        {
            if(sortingNode.MemberPath == StructureStorageSchema.Fields.Id.Name)
            {
                session.SortingParts.Add(string.Format("s.[{0}] {1}", sortingNode.MemberPath, sortingNode.Direction));
                return;
            }

            const string joinFormat = "left join [dbo].[{0}] as siSort{1} on siSort{1}.[{2}] = s.[{3}] and siSort{1}.[{4}]='{5}'";

            var joinTableNum = session.Joins.Count + 1;

            session.Joins.Add(string.Format(joinFormat,
                session.StructureSchema.GetIndexesTableName(),
                joinTableNum,
                IndexStorageSchema.Fields.StructureId.Name,
                StructureStorageSchema.Fields.Id.Name,
                IndexStorageSchema.Fields.MemberPath.Name,
                sortingNode.MemberPath));

            session.SortingParts.Add(string.Format(
                "siSort{0}.[{1}] {2}", joinTableNum, IndexStorageSchema.GetValueSchemaFieldForType(sortingNode.MemberType).Name, sortingNode.Direction));
        }
    }
}