using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Commands;
using SisoDb.Core;
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
            sql.AssertNotNullOrWhiteSpace("sql");
            parameters.AssertNotNull("parameters");

            Sql = sql;

            _parameters = new ReadOnlyCollection<IDacParameter>(parameters.Distinct().ToList());
        }
    }
}