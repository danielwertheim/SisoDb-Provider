using System.Collections.Generic;

namespace SisoDb.Querying.Sql
{
    public interface ISqlExpression 
    {
        IEnumerable<SqlWhereMember> WhereMembers { get; }
        IEnumerable<SqlSortingMember> SortingMembers { get; }
        //TODO: Rem for v16.0.0 final
        //IEnumerable<SqlInclude> Includes { get; }
        SqlWhereCriteria WhereCriteria { get; }
    }
}