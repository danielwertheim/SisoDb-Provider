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
        private readonly ReadOnlyCollection<IDacParameter> _parameters;

        public virtual string Sql { get; private set; }

        public virtual IList<IDacParameter> Parameters
        {
            get { return _parameters; }
        }

        public virtual bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(Sql); }
        }

        public SqlWhere(string sql, IEnumerable<IDacParameter> parameters)
        {
            Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            Sql = sql;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }

        protected SqlWhere()
        {
            Sql = string.Empty;
            _parameters = new ReadOnlyCollection<IDacParameter>(new List<IDacParameter>());
        }

        public static SqlWhere Empty()
        {
            return new SqlWhere();
        }
    }
}