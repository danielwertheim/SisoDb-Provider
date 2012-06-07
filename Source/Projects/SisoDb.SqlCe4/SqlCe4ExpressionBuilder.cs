using System;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlCe4
{
    public class SqlCe4WhereCriteriaBuilder : SqlWhereCriteriaBuilder
    {}

    public class SqlCe4ExpressionBuilder : SqlExpressionBuilder
    {
        public SqlCe4ExpressionBuilder(Func<ISqlWhereCriteriaBuilder> whereCriteriaBuilderFn) 
            : base(whereCriteriaBuilderFn)
        {
        }

        protected override void OnProcessWhereInSetMemberNode(ISqlWhereCriteriaBuilder builder, Querying.Lambdas.Nodes.InSetMemberNode memberNode, int memberIndex)
        {
            base.OnProcessWhereInSetMemberNode(builder, memberNode, memberIndex);
        }
    }
}