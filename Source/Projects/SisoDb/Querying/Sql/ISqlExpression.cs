using System.Collections.Generic;

namespace SisoDb.Querying.Sql
{
    public interface ISqlExpression 
    {
        IEnumerable<SqlWhereMember> WhereMembers { get; }
        IEnumerable<SqlSortingMember> SortingMembers { get; }
        IEnumerable<SqlInclude> Includes { get; }
        SqlWhereCriteria WhereCriteria { get; }
    }
}