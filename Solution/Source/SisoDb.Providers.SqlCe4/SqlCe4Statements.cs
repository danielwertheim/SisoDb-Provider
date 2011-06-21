using System;
using SisoDb.Providers;

namespace SisoDb.SqlCe4
{
    internal class SqlCe4Statements : SqlStatementsBase
    {
        private static readonly Type ThisType;

        internal static readonly ISqlStatements Instance;

        static SqlCe4Statements()
        {
            ThisType = typeof(SqlCe4Statements);
            Instance = new SqlCe4Statements();
        }

        private SqlCe4Statements()
            : base(ThisType.Assembly, "Resources.SqlCe4Statements")
        {
        }
    }
}