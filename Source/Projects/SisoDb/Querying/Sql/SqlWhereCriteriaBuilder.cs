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
    public class SqlWhereCriteriaBuilder : ISqlWhereCriteriaBuilder
    {
        private const string OpMarker = "[%OP%]";
        private const string ValueMarker = "[%VALUE%]";

        protected StringBuilder State;
        protected bool HasWrittenMember;
        protected bool HasWrittenValue;

        public ISet<IDacParameter> Params { get; protected set; }
        
        public bool IsEmpty
        {
            get { return State.Length == 0; }
        }

        public string Sql
        {
            get { return State.ToString(); }
        }

        public SqlWhereCriteriaBuilder()
        {
            State = new StringBuilder();
            Params = new HashSet<IDacParameter>();
        }

        public virtual void Flush()
        {
            if (HasWrittenMember && HasWrittenValue)
            {
                HasWrittenMember = false;
                HasWrittenValue = false;

                State = State.Replace(OpMarker, string.Empty);
            }
        }

        public virtual void AddMember(MemberNode member, int memberIndex)
        {
            Ensure.That(memberIndex, "memberIndex").IsGte(0);

            if (!HasWrittenMember)
            {
                State.AppendFormat(GetMemberFormatString(member),
                    GetMemberNodeString(member, memberIndex),
                    OpMarker,
                    ValueMarker);

                HasWrittenMember = true;
                return;
            }

            //TODO: Perhaps AddMemberAsValue
            if (!HasWrittenValue) //When using member as value, eg. Where Name = SecondName
            {
                State = State.Replace(ValueMarker, GetMemberNodeString(member, memberIndex));
                HasWrittenValue = true;
            }
        }

        public virtual void AddOp(OperatorNode op)
        {
            var opSql = string.Format(op.Operator is NotOperator ? "{0} " : " {0} ", op);
            if (SqlContains(OpMarker))
                State = State.Replace(OpMarker, opSql);
            else
                State.Append(opSql);
        }

        public virtual void AddValue(ValueNode valueNode)
        {
            var param = new DacParameter(GetNextParameterName(), valueNode.Value);
            Params.Add(param);

            AddValue(param.Name);
        }

        public virtual void AddNullValue(NullNode nullNode)
        {
            AddValue("null");
        }

        public virtual void AddSetOfValues(ArrayValueNode valueNode)
        {
            var param = new ArrayDacParameter(GetNextParameterName(), valueNode.MemberDataType, valueNode.MemberDataTypeCode, valueNode.Value);
            Params.Add(param);

            AddValue(string.Concat("(select [Value] from ", param.Name, ")"));
        }

        public virtual void AddRaw(string sql)
        {
            State.Append(sql);
        }

        protected virtual void AddValue(string value)
        {
            if (SqlContains(ValueMarker))
                State = State.Replace(ValueMarker, value);
            else
                State.Append(value);

            HasWrittenValue = true;
        }

        protected string GetNextParameterName()
        {
            return string.Concat("p", Params.Count);
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


        protected virtual string GetMemberFormatString(MemberNode member)
        {
            return "({0}{1}{2})"; // {0} = left, {1} = operator, {2} = right
        }

        protected virtual bool SqlContains(string value)
        {
            return State.ToString().Contains(value);
        }
    }
}