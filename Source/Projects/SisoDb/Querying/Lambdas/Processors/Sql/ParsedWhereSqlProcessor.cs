using System.Collections.Generic;
using System.Text;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public class ParsedWhereSqlProcessor : IParsedLambdaProcessor<ISqlWhere>
    {
        public IMemberNameGenerator MemberNameGenerator { private get; set; }

        public ParsedWhereSqlProcessor(IMemberNameGenerator memberNameGenerator)
        {
            MemberNameGenerator = memberNameGenerator;
        }

        public ISqlWhere Process(IParsedLambda lambda)
        {
            var queryParams = new HashSet<DacParameter>();
            var sql = new StringBuilder();

            foreach (var node in lambda.Nodes)
            {
                if (node is MemberNode)
                {
                    var memPath = ((MemberNode)node).Path;
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
                    var param = new DacParameter(name, valueNode.Value);
                    queryParams.Add(param);
                    sql.Append(param.Name);
                }
                else
                    sql.AppendFormat("{0}", node);
            }

            return new SqlWhere(sql.ToString(), queryParams);
        }
    }
}