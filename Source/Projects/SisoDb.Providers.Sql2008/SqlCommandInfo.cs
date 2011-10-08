using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnsureThat;
using SisoDb.Commands;
using SisoDb.Dac;

namespace SisoDb.Sql2008
{
    [Serializable]
    public class SqlCommandInfo : ISqlCommandInfo
    {
        private readonly ReadOnlyCollection<IDacParameter> _parameters;

        public string Sql { get; private set; }

        public IList<IDacParameter> Parameters
        {
            get { return _parameters; }
        }

        public SqlCommandInfo(string sql, IEnumerable<IDacParameter> parameters)
        {
            Ensure.That(() => sql).IsNotNullOrWhiteSpace();
            Ensure.That(() => parameters).IsNotNull();

            Sql = sql;
            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }
    }
}