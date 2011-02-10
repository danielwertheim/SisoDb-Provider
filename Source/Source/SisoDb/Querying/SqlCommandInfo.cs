using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SisoDb.Querying
{
    [Serializable]
    internal class SqlCommandInfo : ISqlCommandInfo
    {
        private readonly ReadOnlyCollection<IQueryParameter> _parameters;

        public string Value { get; private set; }

        public IEnumerable<IQueryParameter> Parameters
        {
            get { return _parameters; }
        }

        internal SqlCommandInfo(string sql, IEnumerable<IQueryParameter> parameters)
        {
            sql.AssertNotNullOrWhiteSpace("sql");
            parameters.AssertNotNull("parameters");

            Value = sql;

            _parameters = new ReadOnlyCollection<IQueryParameter>(parameters.Distinct().ToList());
        }
    }
}