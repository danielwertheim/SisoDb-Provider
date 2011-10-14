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

        public SqlQuery(string sql, IEnumerable<IDacParameter> parameters)
        {
            Ensure.That(() => sql).IsNotNullOrWhiteSpace();
            Ensure.That(() => parameters).IsNotNull();

            Sql = sql;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }

        protected SqlQuery()
        {
            Sql = string.Empty;
            _parameters = new ReadOnlyCollection<IDacParameter>(new List<IDacParameter>());
        }

        public static SqlQuery Empty()
        {
            return new SqlQuery();
        }

        public override string ToString()
        {
            return Sql;
        }
    }
}