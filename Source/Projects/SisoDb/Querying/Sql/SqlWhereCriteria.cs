using System;
using SisoDb.Dac;
using SisoDb.EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlWhereCriteria
    {
        private readonly string _criteriaString;
        private readonly IDacParameter[] _parameters;
        private readonly bool _isEmpty;

        public virtual string CriteriaString
        {
            get { return _criteriaString; }
        }

        public virtual IDacParameter[] Parameters
        {
            get { return _parameters; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlWhereCriteria(string criteriaString, IDacParameter[] parameters)
        {
            Ensure.That(criteriaString, "criteriaString").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _criteriaString = criteriaString;
            _parameters = parameters;
        }

        private SqlWhereCriteria()
        {
            _isEmpty = true;
            _criteriaString = string.Empty;
            _parameters = new IDacParameter[]{};
        }

        public static SqlWhereCriteria Empty()
        {
            return new SqlWhereCriteria();
        }
    }
}