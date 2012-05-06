using EnsureThat;
using SisoDb.Diagnostics.Appenders;

namespace SisoDb.Diagnostics
{
    public class DbDiagnosticsContextBuilder : IDiagnosticsContextBuilder
    {
        protected readonly ISisoDatabase Database;

        public DbDiagnosticsContextBuilder(ISisoDatabase database)
        {
            Ensure.That(database, "database").IsNotNull();
            Database = database;
        }

        public virtual DiagnosticsContext Build()
        {
            var context = new DiagnosticsContext("SisoDb - Db: {0}", Database.Name);
            return AppendStructureSchemas(context);
        }

        protected virtual DiagnosticsContext AppendStructureSchemas(DiagnosticsContext context)
        {
            var appender = new StructureSchemasAppender(context);
            appender.Append(Database.StructureSchemas);
            return context;
        }
    }
}