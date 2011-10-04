using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlWhere : ISqlWhere
    {
        private readonly ReadOnlyCollection<IDacParameter> _parameters;

        public string Sql { get; private set; }

        public IList<IDacParameter> Parameters
        {
            get { return _parameters; }
        }

        public SqlWhere(string sql, IEnumerable<IDacParameter> parameters)
        {
            Ensure.That(() => sql).IsNotNullOrWhiteSpace();
            Ensure.That(() => parameters).IsNotNull();

            Sql = sql;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }
    }
}