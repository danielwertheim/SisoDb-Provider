using System;
using System.Linq;
using SisoDb.Dac;
using SisoDb.EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class DbQuery : IDbQuery
    {
        private readonly string _sql;
        private readonly IDacParameter[] _parameters;
        private readonly bool _isEmpty;
        private readonly bool _isCacheable;

        public string Sql 
        {
            get { return _sql; }
        }

        public IDacParameter[] Parameters
        {
            get { return _parameters; }
        }

        public bool IsCacheable
        {
            get { return _isCacheable; }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public DbQuery(string sql, IDacParameter[] parameters, bool isCacheable)
        {
            Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _sql = sql;
            _parameters = parameters.Distinct().ToArray();
            _isCacheable = isCacheable;
        }

        protected DbQuery()
        {
            _isEmpty = true;
            _sql = string.Empty;
            _parameters = new IDacParameter[0];
        }

        public static IDbQuery Empty()
        {
            return new DbQuery();
        }
    }
}