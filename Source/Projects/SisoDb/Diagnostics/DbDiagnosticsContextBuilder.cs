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
            var context = new DiagnosticsContext("Db: '{0}'", Database.Name);
            AppendSettings(context);
            AppendStructureSchemas(context);

            return context;
        }

        protected virtual void AppendSettings(DiagnosticsContext context)
        {
            var settingsAppender = new DbSettingsAppender(context);
            settingsAppender.Append(Database.Settings);
        }

        protected virtual void AppendStructureSchemas(DiagnosticsContext context)
        {
            var schemasAppender = new StructureSchemasAppender(context);
            schemasAppender.Append(Database.StructureSchemas);
        }
    }
}