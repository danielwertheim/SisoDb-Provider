using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude
    {
        private readonly string _jsonOutputDefinition;
        private readonly string _joinString;
        private readonly bool _isEmpty;

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

        public SqlInclude(string jsonOutputDefinition, string joinString)
        {
            Ensure.That(jsonOutputDefinition, "jsonOutputDefinition").IsNotNullOrWhiteSpace();
            Ensure.That(joinString, "JoinString").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _jsonOutputDefinition = jsonOutputDefinition;
            _joinString = joinString;
        }

        protected SqlInclude()
        {
            _isEmpty = true;
            _jsonOutputDefinition = string.Empty;
            _joinString = string.Empty;
        }

        public static SqlInclude Empty()
        {
            return new SqlInclude();
        }

        public static string ToColumnDefinitionString(IEnumerable<SqlInclude> includes)
        {
            return string.Join(", ", includes.Select(inc => inc.JsonOutputDefinition));
        }

        public static string ToJoinString(IEnumerable<SqlInclude> includes)
        {
            return string.Join(" ", includes.Select(inc => inc.JoinString));
        }
    }
}