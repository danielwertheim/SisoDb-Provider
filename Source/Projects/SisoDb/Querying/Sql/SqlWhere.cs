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
        private readonly string _sql;
        private readonly ReadOnlyCollection<IDacParameter> _parameters;
        private readonly bool _isEmpty;

        public virtual string Sql
        {
            get { return _sql; }
        }

        public virtual IList<IDacParameter> Parameters
        {
            get { return _parameters; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlWhere(string sql, ICollection<IDacParameter> parameters)
        {
            Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _sql = sql;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }

        protected SqlWhere()
        {
            _isEmpty = true;
            _sql = string.Empty;
            _parameters = new ReadOnlyCollection<IDacParameter>(new List<IDacParameter>());
        }

        public static SqlWhere Empty()
        {
            return new SqlWhere();
        }
    }
}