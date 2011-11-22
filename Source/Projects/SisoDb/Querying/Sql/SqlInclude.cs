using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude
    {
        private readonly string _columnDefinition;
        private readonly string _jsonOutputDefinition;
        private readonly string _joinString;
        private readonly bool _isEmpty;

        public virtual string ColumnDefinition
        {
            get { return _columnDefinition; }
        }

        public virtual string JsonOutputDefinition
        {
            get { return _jsonOutputDefinition; }
        }

        public virtual string JoinString
        {
            get { return _joinString; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlInclude(string columnDefinition, string jsonOutputDefinition, string joinString)
        {
            Ensure.That(columnDefinition, "columnDefinition").IsNotNullOrWhiteSpace();
            Ensure.That(jsonOutputDefinition, "jsonOutputDefinition").IsNotNullOrWhiteSpace();
            Ensure.That(joinString, "JoinString").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _columnDefinition = columnDefinition;
            _jsonOutputDefinition = jsonOutputDefinition;
            _joinString = joinString;
        }

        private SqlInclude()
        {
            _isEmpty = true;
            _columnDefinition = string.Empty;
            _jsonOutputDefinition = string.Empty;
            _joinString = string.Empty;
        }

        public static SqlInclude Empty()
        {
            return new SqlInclude();
        }
    }
}