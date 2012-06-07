using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Sql
{
    /// <summary>
    /// Builds the Where part of the SQL queries.
    /// </summary>
    public interface ISqlWhereCriteriaBuilder
    {
        ISet<IDacParameter> Params { get; }
        bool IsEmpty { get; }
        string Sql { get; }

        void Flush();
        void AddMember(MemberNode member, int memberIndex);
        void AddOp(OperatorNode op);
        void AddValue(ValueNode valueNode);
        void AddNullValue(NullNode nullNode);
        void AddSetOfValues(ArrayValueNode valueNode);
        void AddRaw(string sql);
    }
}