using EnsureThat;
using SisoDb.Diagnostics.Appenders;

namespace SisoDb.Diagnostics
{
    public class DbDiagnosticsSectionBuilder : IDiagnosticsSectionBuilder
    {
        protected readonly ISisoDatabase Database;

        public DbDiagnosticsSectionBuilder(ISisoDatabase database)
        {
            Ensure.That(database, "database").IsNotNull();
            Database = database;
        }

        public virtual DiagnosticsSection Build()
        {
            var context = new DiagnosticsSection("Db: '{0}'", Database.Name);
            AppendSettings(context);
            AppendStructureSchemas(context);

            return context;
        }

        protected virtual void AppendSettings(DiagnosticsSection section)
        {
            var settingsAppender = new DbSettingsSectionAppender(section);
            settingsAppender.Append(Database.Settings);
        }

        protected virtual void AppendStructureSchemas(DiagnosticsSection section)
        {
            var schemasAppender = new StructureSchemasSectionAppender(section);
            schemasAppender.Append(Database.StructureSchemas);
        }
    }
}