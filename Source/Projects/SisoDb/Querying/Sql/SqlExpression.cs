using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlExpression : ISqlExpression
    {
        private readonly List<string> _allMemberPaths = new List<string>(); 
        private readonly List<SqlWhereMember> _whereMembers = new List<SqlWhereMember>();
        private readonly List<SqlSortingMember> _sortingMembers = new List<SqlSortingMember>();
        private readonly List<SqlInclude> _includes = new List<SqlInclude>(); 
        private SqlWhereCriteria _whereCriteria = SqlWhereCriteria.Empty();

        public virtual IEnumerable<SqlWhereMember> WhereMembers
        {
            get { return _whereMembers; }
        }

        public virtual IEnumerable<SqlSortingMember> SortingMembers
        {
            get { return _sortingMembers; }
        }

        public virtual IEnumerable<SqlInclude> Includes
        {
            get { return _includes; }
        }

        public virtual SqlWhereCriteria WhereCriteria
        {
            get { return _whereCriteria; }
        }

        public virtual int GetExistingOrNewMemberIndexFor(string memberPath)
        {
            var index = _allMemberPaths.IndexOf(memberPath);

            return index > -1 
                ? index 
                : _allMemberPaths.Count;
        }

        public virtual int GetNextNewIncludeIndex()
        {
            return _includes.Count;
        }

        public virtual bool ContainsWhereMemberFor(string memberPath)
        {
            return _whereMembers.Any(m => m.MemberPath == memberPath);
        }

        public virtual bool ContainsSortingMemberFor(string memberPath)
        {
            return _sortingMembers.Any(m => m.MemberPath == memberPath);
        }

        public virtual void AddWhereMember(SqlWhereMember whereMember)
        {
            Ensure.That(whereMember, "whereMember").IsNotNull();

            if (!ContainsWhereMemberFor(whereMember.MemberPath))
            {
                _whereMembers.Add(whereMember);
                AddMemberPath(whereMember.MemberPath);
            }
        }

        public virtual void AddSortingMember(SqlSortingMember sortingMember)
        {
            Ensure.That(sortingMember, "sortingMember").IsNotNull();

            if (!ContainsSortingMemberFor(sortingMember.MemberPath))
            {
                _sortingMembers.Add(sortingMember);
                AddMemberPath(sortingMember.MemberPath);
            }
        }

        private void AddMemberPath(string memberPath)
        {
            if(_allMemberPaths.IndexOf(memberPath) > -1)
                return;

            _allMemberPaths.Add(memberPath);
        }

        public virtual void SetWhereCriteria(SqlWhereCriteria whereCriteria)
        {
            Ensure.That(whereCriteria, "whereCriteria").IsNotNull();

            _whereCriteria = whereCriteria;
        }

        public virtual void AddInclude(SqlInclude include)
        {
            Ensure.That(include, "include").IsNotNull();

            _includes.Add(include);
        }
    }
}