using System;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public class ParsedWhereSqlProcessor : IParsedLambdaProcessor<ISqlWhere>
    {
        public ISqlWhere Process(IParsedLambda lambda)
        {
            var sqlBuilder = new SqlBuilder();

            foreach (var node in lambda.Nodes)
            {
                if (node is MemberNode)
                    sqlBuilder.AddMember((MemberNode)node);
                else if (node is OperatorNode)
                    sqlBuilder.AddOp((OperatorNode)node);
                else if (node is ValueNode)
                    sqlBuilder.AddValue((ValueNode)node);
                else
                    sqlBuilder.AddOther(node);

                sqlBuilder.Flush();
            }

            return new SqlWhere(sqlBuilder.Sql, sqlBuilder.Params);
        }
    }
}