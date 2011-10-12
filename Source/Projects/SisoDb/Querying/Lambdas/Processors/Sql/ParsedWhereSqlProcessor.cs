using System;
using System.Collections.Generic;
using System.Text;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public class SqlBuilder
    {
        private readonly StringBuilder _sql = new StringBuilder();

        private INode _left;
        private INode _op;
        private INode _right;

        public void AddMember(MemberNode node)
        {}
    }

    public class ParsedWhereSqlProcessor : IParsedLambdaProcessor<ISqlWhere>
    {
        public IMemberPathGenerator MemberPathGenerator { private get; set; }

        public ParsedWhereSqlProcessor(IMemberPathGenerator memberPathGenerator)
        {
            MemberPathGenerator = memberPathGenerator;
        }

        public ISqlWhere Process(IParsedLambda lambda)
        {
            var queryParams = new HashSet<DacParameter>();
            var sql = new StringBuilder();

            var hasWrittenMemberNode = false;
            var hasWrittenValueNode = false;
            var hasWrittenOpNode = false;

            Action reset = () =>
            {
                hasWrittenMemberNode = false;
                hasWrittenOpNode = false;
                hasWrittenValueNode = false;
            };

            foreach (var node in lambda.Nodes)
            {
                if (node is MemberNode)
                {
                    var member = (MemberNode)node;

                    if (!hasWrittenMemberNode)
                    {
                        if(member.Name == StructureSchema.IdMemberName)
                        {
                            sql.AppendFormat("si.[{0}] [%OP%] [%VALUE%]", StructureSchema.IdMemberName);

                            hasWrittenMemberNode = true;

                            continue;
                        }
                        
                        sql.AppendFormat("(si.[MemberPath]='{0}' and si.[{1}] [%OP%] [%VALUE%])",
                                         MemberPathGenerator.Generate(member.Path),
                                         IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name);

                        hasWrittenMemberNode = true;

                        continue;
                    }

                    if(!hasWrittenValueNode)
                    {
                        if (member.Name == StructureSchema.IdMemberName)
                        {
                            sql = sql.Replace("[%VALUE%]", string.Format("si.[{0}]", StructureSchema.IdMemberName));

                            hasWrittenMemberNode = true;

                            continue;
                        }

                        sql = sql.Replace("[%VALUE%]", string.Format("(select sub.[{0}] from CustomerIndexes sub where sub.RowId = si.RowId)",
                                         IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name));

                        hasWrittenValueNode = true;
                    }
                }
                else if (node is OperatorNode)
                {
                    var op = node as OperatorNode;
                    if (op.Operator is NotOperator)
                        sql = sql.Replace("[%OP%]",string.Format("{0} ", node));
                    else
                        sql = sql.Replace("[%OP%]", string.Format(" {0} ", node));

                    hasWrittenOpNode = true;
                }
                else if (node is ValueNode)
                {
                    var valueNode = (ValueNode)node;
                    var name = "@p" + queryParams.Count;
                    var param = new DacParameter(name, valueNode.Value);
                    queryParams.Add(param);
                    sql = sql.Replace("[%VALUE%]", string.Format(" {0} ", param.Name));

                    hasWrittenValueNode = true;
                }
                else
                    sql.AppendFormat("{0}", node);

                if (hasWrittenMemberNode && hasWrittenValueNode)
                    reset();
            }

            if (!hasWrittenOpNode)
                sql = sql.Replace("[%OP%]", string.Empty);

            if(!hasWrittenValueNode)
                sql = sql.Replace("[%VALUE%]", string.Empty);

            return new SqlWhere(sql.ToString(), queryParams);
        }
    }
}