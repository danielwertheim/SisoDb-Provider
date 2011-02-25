using System.Collections.Generic;
using System.Text;
using SisoDb.Lambdas.Nodes;
using SisoDb.Lambdas.Operators;
using SisoDb.Querying;

namespace SisoDb.Lambdas.Processors
{
    public class ParsedSelectorSqlProcessor : IParsedLambdaProcessor<ISqlSelector>
    {
        private static readonly INameStrategy NameStrategy = new NameStrategy();

        public IMemberNameGenerator MemberNameGenerator { private get; set; }

        public ParsedSelectorSqlProcessor(IMemberNameGenerator memberNameGenerator)
        {
            MemberNameGenerator = memberNameGenerator;
        }

        public ISqlSelector Process(IParsedLambda lambda)
        {
            var queryParams = new HashSet<QueryParameter>();
            var sql = new StringBuilder();

            foreach (var node in lambda.Nodes)
            {
                if (node is MemberNode)
                {
                    var memNode = (MemberNode)node;
                    var memPath = NameStrategy.Apply(memNode.Path);
                    sql.AppendFormat("si.[{0}]", MemberNameGenerator.Generate(memPath));
                }
                else if (node is OperatorNode)
                {
                    var op = node as OperatorNode;
                    if (op.Operator is NotOperator)
                        sql.AppendFormat("{0} ", node);
                    else
                        sql.AppendFormat(" {0} ", node);
                }
                else if (node is ValueNode)
                {
                    var valueNode = (ValueNode)node;
                    var name = "@p" + queryParams.Count;
                    var param = new QueryParameter(name, valueNode.Value);
                    queryParams.Add(param);
                    sql.Append(param.Name);
                }
                else
                    sql.AppendFormat("{0}", node);
            }

            return new SqlSelector(sql.ToString(), queryParams);
        }
    }
}