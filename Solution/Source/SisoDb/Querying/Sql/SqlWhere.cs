using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Core;
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
            sql.AssertNotNullOrWhiteSpace("sql");
            parameters.AssertNotNull("parameters");

            Sql = sql;

            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }
    }
}