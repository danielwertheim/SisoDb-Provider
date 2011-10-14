using System;
using System.Text;
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
        public SqlSorting Convert(IStructureSchema structureSchema, IParsedLambda lambda)
        {
            var sql = new StringBuilder();
            var joinFormat = "left join dbo.[{0}] siSort1 on siSort1.[{1}] = s.[{2}]] and siSort1.[{3}]='{4}'";
            var orderByFormat = "order by si1.StringValue, si2.integervalue desc";

            var lastIndex = lambda.Nodes.Count - 1;
            for (var i = 0; i < lambda.Nodes.Count; i++)
            {
                var node = lambda.Nodes[i];

                if (node is SortingNode)
                {
                    var sortingNode = (SortingNode) node;
                    sql.AppendFormat("si.[{0}]='{1}' {2}", 
                        IndexStorageSchema.Fields.MemberPath.Name,
                        sortingNode.MemberPath, sortingNode.Direction);

                    if (i != lastIndex)
                        sql.Append(", ");

                    continue;
                }

                throw new NotSupportedException(ExceptionMessages.
                    ParsedSortingLambdaSqlProcessor_NotSupportedNodeType.Inject(node.GetType().Name));
            }

            return new SqlSorting(sql.ToString());
        }
    }
}