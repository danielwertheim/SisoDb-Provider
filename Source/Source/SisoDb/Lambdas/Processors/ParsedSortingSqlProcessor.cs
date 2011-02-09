using System;
using System.Text;
using SisoDb.Lambdas.Nodes;
using SisoDb.Querying;
using SisoDb.Resources;

namespace SisoDb.Lambdas.Processors
{
    internal class ParsedSortingSqlProcessor : IParsedLambdaProcessor<ISqlSorting>
    {
        private readonly IMemberNameGenerator _memberNameGenerator;

        public ParsedSortingSqlProcessor(IMemberNameGenerator memberNameGenerator)
        {
            _memberNameGenerator = memberNameGenerator;
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
                    sql.AppendFormat("si.[{0}] {1}", _memberNameGenerator.Generate(sortingNode.Name), sortingNode.Direction);

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