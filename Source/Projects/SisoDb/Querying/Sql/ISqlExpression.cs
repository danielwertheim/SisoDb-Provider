using System.Collections.Generic;

namespace SisoDb.Querying.Sql
{
    public interface ISqlExpression 
    {
        IEnumerable<SqlWhereMember> WhereMembers { get; }
        IEnumerable<SqlSortingMember> SortingMembers { get; }
        SqlWhereCriteria WhereCriteria { get; }
    }
}