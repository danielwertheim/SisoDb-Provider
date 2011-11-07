using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlWhere
    {
        private readonly string _memberPath;
        private readonly string _joinString;
        private readonly string _criteriaString;
        private readonly ReadOnlyCollection<IDacParameter> _parameters;
        private readonly bool _isEmpty;

        public virtual string MemberPath
        {
            get { return _memberPath; }
        }

        public virtual string JoinString
        {
            get { return _joinString; }
        }

        public virtual string CriteriaString
        {
            get { return _criteriaString; }
        }

        public virtual IList<IDacParameter> Parameters
        {
            get { return _parameters; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlWhere(string memberPath, string joinString, string criteriaString, ICollection<IDacParameter> parameters)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(joinString, "joinString").IsNotNullOrWhiteSpace();
            Ensure.That(criteriaString, "criteriaString").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _memberPath = memberPath;
            _joinString = joinString;
            _criteriaString = criteriaString;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }

        protected SqlWhere()
        {
            _isEmpty = true;
            _memberPath = string.Empty;
            _joinString = string.Empty;
            _criteriaString = string.Empty;
            _parameters = new ReadOnlyCollection<IDacParameter>(new List<IDacParameter>());
        }

        public static SqlWhere Empty()
        {
            return new SqlWhere();
        }

        public static string ToMemberPathString(IEnumerable<SqlWhere> wheres, string decorateSortingWith = null)
        {
            if(string.IsNullOrWhiteSpace(decorateSortingWith))
                return string.Join(", ", wheres.Select(w => w.MemberPath));

            return string.Join(", ", wheres.Select(w => string.Format(decorateSortingWith, w.MemberPath)));
        }

        public static string ToJoinString(IEnumerable<SqlWhere> wheres)
        {
            return string.Join(" ", wheres.Select(w => w.JoinString));
        }
    }
}