using System;
using System.Text;
using SisoDb.Core;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public class ParsedSortingSqlProcessor : IParsedLambdaProcessor<ISqlSorting>
    {
        public IMemberNameGenerator MemberNameGenerator { private get; set; }

        public ParsedSortingSqlProcessor(IMemberNameGenerator memberNameGenerator)
        {
            MemberNameGenerator = memberNameGenerator.AssertNotNull("memberNameGenerator");
        }

        public ISqlSorting Process(IParsedLambda lambda)
        {
            var sql = new StringBuilder();

            var lastIndex = lambda.Nodes.Count - 1;
            for (var i = 0; i < lambda.Nodes.Count; i++)
            {
                var node = lambda.Nodes[i];

                if (node is SortingNode)
                {
                    var sortingNode = (SortingNode) node;
                    sql.AppendFormat("si.[{0}] {1}", MemberNameGenerator.Generate(sortingNode.MemberPath), sortingNode.Direction);

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