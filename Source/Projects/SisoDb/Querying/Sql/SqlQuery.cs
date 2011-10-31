using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlQuery
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

        public SqlQuery(string sql, ICollection<IDacParameter> parameters)
        {
            Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _sql = sql;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }

        protected SqlQuery()
        {
            _isEmpty = true;
            _sql = string.Empty;
            _parameters = new ReadOnlyCollection<IDacParameter>(new List<IDacParameter>());
        }

        public static SqlQuery Empty()
        {
            return new SqlQuery();
        }
    }
}