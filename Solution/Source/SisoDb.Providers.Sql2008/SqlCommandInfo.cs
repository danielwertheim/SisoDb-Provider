using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Commands;
using SisoDb.Core;
using SisoDb.Querying;

namespace SisoDb.Providers.Sql2008
{
    [Serializable]
    public class SqlCommandInfo : ISqlCommandInfo
    {
        private readonly ReadOnlyCollection<IQueryParameter> _parameters;

        public string Sql { get; private set; }

        public IList<IQueryParameter> Parameters
        {
            get { return _parameters; }
        }

        public SqlCommandInfo(string sql, IEnumerable<IQueryParameter> parameters)
        {
            sql.AssertNotNullOrWhiteSpace("sql");
            parameters.AssertNotNull("parameters");

            Sql = sql;

            _parameters = new ReadOnlyCollection<IQueryParameter>(parameters.Distinct().ToList());
        }
    }
}