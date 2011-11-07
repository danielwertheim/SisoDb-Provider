using System.Collections.Generic;
using System.Text;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.Querying.Lambdas.Converters.Sql
{
    public class LambdaToSqlWhereConverter : ILambdaToSqlWhereConverter
    {
        private class Session
        {
            public const string OpMarker = "[%OP%]";
            public const string ValueMarker = "[%VALUE%]";

            public readonly IStructureSchema StructureSchema;
            public StringBuilder Sql = new StringBuilder();
            public readonly ISet<IDacParameter> Params;
            public string PreviousMemberPath;
            public bool HasWrittenMember;
            public bool HasWrittenValue;

            public Session(IStructureSchema structureSchema)
            {
                StructureSchema = structureSchema;
                Params = new HashSet<IDacParameter>();
            }

            public void Flush()
            {
                if(HasWrittenMember && HasWrittenValue)
                {
                    PreviousMemberPath = null;
                    HasWrittenMember = false;
                    HasWrittenValue = false;

                    Sql = Sql.Replace(OpMarker, string.Empty);
                    Sql = Sql.Replace(ValueMarker, string.Empty);
                }
            }

            public bool SqlContains(string value)
            {
                return Sql.ToString().Contains(value);
            }
        }

        public IList<SqlWhere> Convert(IStructureSchema structureSchema, IParsedLambda lambda)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var wheres = new List<SqlWhere>();

            if (lambda == null)
                return wheres;

            var session = new Session(structureSchema);

            foreach (var node in lambda.Nodes)
            {
                if (node is MemberNode)
                    AddMember(session, (MemberNode)node);
                else if (node is OperatorNode)
                    AddOp(session, (OperatorNode)node);
                else if (node is ValueNode)
                    AddValue(session, (ValueNode)node);
                else if (node is NullNode)
                    AddNullValue(session, (NullNode)node);
                else
                    session.Sql.Append(node);

                session.Flush();
            }

            return wheres;
            //return session.Sql.Length > 0 ? new SqlWhere(session.Sql.ToString(), session.Params) : SqlWhere.Empty(); 
        }

        private static void AddMember(Session session, MemberNode member)
        {
            if (!session.HasWrittenMember)
            {
                if (member.Path == StructureSchema.IdMemberName)
                {
                    session.Sql.AppendFormat("si.[{0}]{1}{2}", StructureSchema.IdMemberName, Session.OpMarker, Session.ValueMarker);

                    session.PreviousMemberPath = member.Path;
                    session.HasWrittenMember = true;

                    return;
                }

                session.Sql.AppendFormat("(si.[{0}]='{1}' and si.[{2}]{3}{4})",
                    IndexStorageSchema.Fields.MemberPath.Name,
                    member.Path,
                    IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name,
                    Session.OpMarker,
                    Session.ValueMarker);

                session.PreviousMemberPath = member.Path;
                session.HasWrittenMember = true;

                return;
            }

            if (!session.HasWrittenValue)
            {
                if (member.Path == StructureSchema.IdMemberName)
                {
                    session.Sql = session.Sql.Replace(Session.ValueMarker, string.Format("si.[{0}]", StructureSchema.IdMemberName));
                    
                    session.HasWrittenValue = true;

                    return;
                }

                if(member.Path == session.PreviousMemberPath)
                    session.Sql = session.Sql.Replace(Session.ValueMarker,
                        string.Format("(select sub.[{0}] from [{1}] sub where sub.[RowId]=si.[RowId])",
                            IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name,
                            session.StructureSchema.GetIndexesTableName()));
                else
                    session.Sql = session.Sql.Replace(Session.ValueMarker,
                    string.Format("(select sub.[{0}] from [{1}] sub where sub.[MemberPath]='{2}')",
                        IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name,
                        session.StructureSchema.GetIndexesTableName(),
                        member.Path));

                session.HasWrittenValue = true;
            }
        }

        private static void AddOp(Session session, OperatorNode op)
        {
            var opSql = string.Format(op.Operator is NotOperator ? "{0} " : " {0} ", op);
            if (session.SqlContains("[%OP%]"))
                session.Sql = session.Sql.Replace("[%OP%]", opSql);
            else
                session.Sql.Append(opSql);
        }

        private static void AddValue(Session session, ValueNode valueNode)
        {
            var name = "@p" + session.Params.Count;
            var param = new DacParameter(name, valueNode.Value);
            session.Params.Add(param);

            if (session.SqlContains(Session.ValueMarker))
                session.Sql = session.Sql.Replace(Session.ValueMarker, param.Name);
            else
                session.Sql.Append(param.Name);

            session.HasWrittenValue = true;
        }

        private void AddNullValue(Session session, NullNode nullNode)
        {
            if (session.SqlContains(Session.ValueMarker))
                session.Sql = session.Sql.Replace(Session.ValueMarker, nullNode.ToString());
            else
                session.Sql.Append(nullNode.ToString());

            session.HasWrittenValue = true;
        }
    }
}