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
        private readonly string[] _memberPaths;
        private readonly string _criteriaString;
        private readonly ReadOnlyCollection<IDacParameter> _parameters;
        private readonly bool _isEmpty;

        public virtual string[] MemberPaths
        {
            get { return _memberPaths; }
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

        public SqlWhere(string[] memberPaths, string criteriaString, ICollection<IDacParameter> parameters)
        {
            Ensure.That(memberPaths, "memberPaths").HasItems();
            Ensure.That(criteriaString, "criteriaString").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _memberPaths = memberPaths;
            _criteriaString = criteriaString;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }

        protected SqlWhere()
        {
            _isEmpty = true;
            _memberPaths = new string[] { };
            _criteriaString = string.Empty;
            _parameters = new ReadOnlyCollection<IDacParameter>(new List<IDacParameter>());
        }

        public static SqlWhere Empty()
        {
            return new SqlWhere();
        }
    }
}