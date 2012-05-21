using System.Collections.Generic;
using System.Text;
using EnsureThat;
using NCore.Reflections;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Sql
{
    internal class WhereCriteriaBuilder
    {
        internal const string OpMarker = "[%OP%]";
        internal const string ValueMarker = "[%VALUE%]";

        internal StringBuilder Sql = new StringBuilder();
        internal readonly ISet<IDacParameter> Params;
        internal bool HasWrittenMember;
        internal bool HasWrittenValue;

        internal WhereCriteriaBuilder()
        {
            Params = new HashSet<IDacParameter>();
        }

        internal void Flush()
        {
            if (HasWrittenMember && HasWrittenValue)
            {
                HasWrittenMember = false;
                HasWrittenValue = false;

                Sql = Sql.Replace(OpMarker, string.Empty);
                Sql = Sql.Replace(ValueMarker, string.Empty);
            }
        }

        private bool SqlContains(string value)
        {
            return Sql.ToString().Contains(value);
        }

        internal void AddMember(MemberNode member, int memberIndex)
        {
            Ensure.That(memberIndex, "memberIndex").IsGte(0);

            if (!HasWrittenMember)
            {
                Sql.AppendFormat("({0}{1}{2})",
                    GetMemberNodeString(member, memberIndex),
                    OpMarker,
                    ValueMarker);

                HasWrittenMember = true;

                return;
            }

            if (!HasWrittenValue)
            {
                Sql = Sql.Replace(ValueMarker,
                    string.Format("({0}{1}{2})",
                    GetMemberNodeString(member, memberIndex),
                    OpMarker,
                    ValueMarker));

                HasWrittenValue = true;
            }
        }

        private static string GetMemberNodeString(MemberNode member, int memberIndex)
        {
            var memFormat = "mem{0}.[{1}]";

            if (member is ToLowerMemberNode)
                memFormat = string.Format("lower({0})", memFormat);

            if (member is ToUpperMemberNode)
                memFormat = string.Format("upper({0})", memFormat);

        	if ((member is StartsWithMemberNode || member is EndsWithMemberNode) && !(member.DataType.IsStringType() || member.DataType.IsAnyEnumType()))
        		return string.Format(memFormat, memberIndex, IndexStorageSchema.Fields.StringValue.Name);

        	return string.Format(memFormat, memberIndex, IndexStorageSchema.Fields.Value.Name);
        }

        internal void AddOp(OperatorNode op)
        {
            var opSql = string.Format(op.Operator is NotOperator ? "{0} " : " {0} ", op);
            if (SqlContains("[%OP%]"))
                Sql = Sql.Replace("[%OP%]", opSql);
            else
                Sql.Append(opSql);
        }

        internal void AddValue(ValueNode valueNode)
        {
            var param = new DacParameter(string.Concat("p", Params.Count), valueNode.Value);
            Params.Add(param);

            if (SqlContains(ValueMarker))
                Sql = Sql.Replace(ValueMarker, param.Name);
            else
                Sql.Append(param.Name);

            HasWrittenValue = true;
        }

        internal void AddNullValue(NullNode nullNode)
        {
            if (SqlContains(ValueMarker))
                Sql = Sql.Replace(ValueMarker, nullNode.ToString());
            else
                Sql.Append(nullNode);

            HasWrittenValue = true;
        }
    }
}