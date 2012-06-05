using System;
using System.Collections.Generic;
using System.Text;
using EnsureThat;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Structures;

namespace SisoDb.Querying.Sql
{
    public class WhereCriteriaBuilder
    {
        public const string OpMarker = "[%OP%]";
        public const string ValueMarker = "[%VALUE%]";

        public StringBuilder Sql { get; protected set; }
        public ISet<IDacParameter> Params { get; protected set; }
        public bool HasWrittenMember { get; set; }
        public bool HasWrittenValue { get; set; }

        public WhereCriteriaBuilder()
        {
            Sql = new StringBuilder();
            Params = new HashSet<IDacParameter>();
        }

        public virtual void Flush()
        {
            if (HasWrittenMember && HasWrittenValue)
            {
                HasWrittenMember = false;
                HasWrittenValue = false;

                Sql = Sql.Replace(OpMarker, string.Empty);
                Sql = Sql.Replace(ValueMarker, string.Empty);
            }
        }

        protected virtual bool SqlContains(string value)
        {
            return Sql.ToString().Contains(value);
        }

        public virtual void AddMember(MemberNode member, int memberIndex)
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

        protected virtual string GetMemberNodeString(MemberNode member, int memberIndex)
        {
            var memFormat = "mem{0}.[{1}]";

            if (member is ToLowerMemberNode)
                memFormat = string.Format("lower({0})", memFormat);

            if (member is ToUpperMemberNode)
                memFormat = string.Format("upper({0})", memFormat);

            var shouldUseExplicitStringValue = member is IStringOperationMemberNode && member.DataTypeCode.IsValueType();
        	return shouldUseExplicitStringValue
                ? string.Format(memFormat, memberIndex, IndexStorageSchema.Fields.StringValue.Name)
                : string.Format(memFormat, memberIndex, IndexStorageSchema.Fields.Value.Name);
        }

        public virtual void AddOp(OperatorNode op)
        {
            var opSql = string.Format(op.Operator is NotOperator ? "{0} " : " {0} ", op);
            AppendOperator(opSql);
        }

        public virtual void AddOpWithContainedValues(OperatorNode op, Action valueAppender)
        {
            AppendOperator("()");
        }

        protected virtual void AppendOperator(string opSql)
        {
            if (SqlContains("[%OP%]"))
                Sql = Sql.Replace("[%OP%]", opSql);
            else
                Sql.Append(opSql);
        }

        public virtual void AddValue(ValueNode valueNode)
        {
            var param = new DacParameter(string.Concat("p", Params.Count), valueNode.Value);
            Params.Add(param);

            if (SqlContains(ValueMarker))
                Sql = Sql.Replace(ValueMarker, param.Name);
            else
                Sql.Append(param.Name);

            HasWrittenValue = true;
        }

        public virtual void AddValues(ArrayValueNode valueNode)
        {
            var param = new ArrayDacParameter(string.Concat("p", Params.Count), valueNode.Value);
            Params.Add(param);

            if (SqlContains(ValueMarker))
                Sql = Sql.Replace(ValueMarker, param.Name);
            else
                Sql.Append(param.Name);

            HasWrittenValue = true;
        }

        public virtual void AddNullValue(NullNode nullNode)
        {
            if (SqlContains(ValueMarker))
                Sql = Sql.Replace(ValueMarker, nullNode.ToString());
            else
                Sql.Append(nullNode);

            HasWrittenValue = true;
        }
    }
}