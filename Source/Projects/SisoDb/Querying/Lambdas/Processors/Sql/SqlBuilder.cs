using System.Collections.Generic;
using System.Text;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public class SqlBuilder
    {
        private readonly HashSet<DacParameter> _queryParams = new HashSet<DacParameter>();
        private StringBuilder _sql = new StringBuilder();
        private INode _member;
        private INode _op;
        private INode _value;

        private INode Member
        {
            get { return _member; }
            set
            {
                _member = value;
                OnAfterAdd();
            }
        }

        private INode Op
        {
            get { return _op; }
            set
            {
                _op = value;
                OnAfterAdd();
            }
        }

        private INode Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnAfterAdd();
            }
        }

        private bool HasWrittenMember
        {
            get { return Member != null; }
        }

        private bool HasWrittenOp
        {
            get { return Op != null; }
        }

        private bool HasWrittenValue
        {
            get { return Value != null; }
        }

        public string Sql
        {
            get { return _sql.ToString(); }
        }

        public IEnumerable<DacParameter> Params
        {
            get { return _queryParams; }
        }

        public void AddOther(INode node)
        {
            _sql.AppendFormat("{0}", node);
        }

        public void AddMember(MemberNode member)
        {
            if (!HasWrittenMember)
            {
                if (member.Name == StructureSchema.IdMemberName)
                {
                    _sql.AppendFormat("si.[{0}] [%OP%] [%VALUE%]", StructureSchema.IdMemberName);

                    Member = member;

                    return;
                }

                _sql.AppendFormat("(si.[MemberPath]='{0}' and si.[{1}] [%OP%] [%VALUE%])",
                                  member.Path,
                                  IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name);

                Member = member;

                return;
            }

            if (!HasWrittenValue)
            {
                if (member.Name == StructureSchema.IdMemberName)
                {
                    _sql = _sql.Replace("[%VALUE%]", string.Format("si.[{0}]", StructureSchema.IdMemberName));

                    Value = member;

                    return;
                }

                _sql = _sql.Replace("[%VALUE%]", string.Format("(select sub.[{0}] from CustomerIndexes sub where sub.RowId = si.RowId)",
                                                               IndexStorageSchema.GetValueSchemaFieldForType(member.MemberType).Name));

                Value = member;
            }
        }

        public void AddOp(OperatorNode op)
        {
            if (op.Operator is NotOperator)
                _sql = _sql.Replace("[%OP%]", string.Format("{0} ", op));
            else
                _sql = _sql.Replace("[%OP%]", string.Format(" {0} ", op));

            Op = op;

            OnAfterAdd();
        }

        public void AddValue(ValueNode valueNode)
        {
            var name = "@p" + _queryParams.Count;
            var param = new DacParameter(name, valueNode.Value);

            _queryParams.Add(param);
            _sql = _sql.Replace("[%VALUE%]", string.Format(" {0} ", param.Name));

            Value = valueNode;

            OnAfterAdd();
        }

        private void OnAfterAdd()
        {
            if (HasWrittenMember && HasWrittenValue)
            {
                Member = null;
                Op = null;
                Value = null;
            }
        }

        public void Flush()
        {
            if (!HasWrittenOp)
                _sql = _sql.Replace("[%OP%]", string.Empty);

            if (!HasWrittenValue)
                _sql = _sql.Replace("[%VALUE%]", string.Empty);
        }
    }
}