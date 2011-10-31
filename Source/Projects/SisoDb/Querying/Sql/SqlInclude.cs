using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude
    {
        private readonly string _jsonOutputDefinition;
        private readonly string _join;
        private readonly bool _isEmpty;

        public virtual string JsonOutputDefinition
        {
            get { return _jsonOutputDefinition; }
        }

        public virtual string Join
        {
            get { return _join; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlInclude(string jsonOutputDefinition, string @join)
        {
            Ensure.That(jsonOutputDefinition, "jsonOutputDefinition").IsNotNullOrWhiteSpace();
            Ensure.That(@join, "join").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _jsonOutputDefinition = jsonOutputDefinition;
            _join = @join;
        }

        protected SqlInclude()
        {
            _isEmpty = true;
            _jsonOutputDefinition = string.Empty;
            _join = string.Empty;
        }

        public static SqlInclude Empty()
        {
            return new SqlInclude();
        }
    }
}