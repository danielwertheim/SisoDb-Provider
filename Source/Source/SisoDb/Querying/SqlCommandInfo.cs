using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SisoDb.Querying
{
    [Serializable]
    public class SqlCommandInfo : ISqlCommandInfo
    {
        private readonly ReadOnlyCollection<IQueryParameter> _parameters;

        public string Value { get; private set; }

        public IList<IQueryParameter> Parameters
        {
            get { return _parameters; }
        }

        public SqlCommandInfo(string sql, IEnumerable<IQueryParameter> parameters)
        {
            sql.AssertNotNullOrWhiteSpace("sql");
            parameters.AssertNotNull("parameters");

            Value = sql;

            _parameters = new ReadOnlyCollection<IQueryParameter>(parameters.Distinct().ToList());
        }
    }
}