using System;
using SisoDb.Providers;

namespace SisoDb.SqlCe4
{
    internal class SqlCe4Statements : SqlStatementsBase
    {
        private static readonly Type ThisType;

        static SqlCe4Statements()
        {
            ThisType = typeof(SqlCe4Statements);
        }

        internal SqlCe4Statements()
            : base(ThisType.Assembly, "Resources.SqlCe4Statements")
        {
        }
    }
}