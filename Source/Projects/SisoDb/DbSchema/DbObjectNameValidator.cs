using System.Text.RegularExpressions;
using EnsureThat;

namespace SisoDb.DbSchema
{
    public static class DbObjectNameValidator
    {
        private static readonly Regex ValidDbObjectNameRegEx;

        static DbObjectNameValidator()
        {
            ValidDbObjectNameRegEx = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
        }

        public static void EnsureValid(string dbObjectName)
        {
            Ensure.That(dbObjectName, "dbObjectName").IsNotNullOrWhiteSpace();
            Ensure.That(dbObjectName, "dbObjectName").Matches(ValidDbObjectNameRegEx);
        }
    }
}