using System;
using SisoDb.Providers;

namespace SisoDb.Sql2008
{
    internal class Sql2008Statements : SqlStatementsBase
    {
        private static readonly Type ThisType;

        static Sql2008Statements()
        {
            ThisType = typeof (Sql2008Statements);
        }

        internal Sql2008Statements() 
            : base(ThisType.Assembly, "Resources.Sql2008Statements")
        {
        }
    }
}