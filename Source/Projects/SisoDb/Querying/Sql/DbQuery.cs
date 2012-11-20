using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public virtual string Sql 
        {
            get { return _sql; }
        }

        public virtual IDacParameter[] Parameters
        {
            get { return _parameters; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public DbQuery(string sql, IDacParameter[] parameters)
        {
            Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
            Ensure.That(parameters, "parameters").IsNotNull();

            _isEmpty = false;
            _sql = sql;
            _parameters = parameters.Distinct().ToArray();
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