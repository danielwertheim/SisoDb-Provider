namespace SisoDb.Diagnostics.Appenders
{
    public class DbSettingsSectionAppender : IDiagnosticsSectionAppender<IDbSettings>
    {
        protected readonly DiagnosticsSection Section;

        public DbSettingsSectionAppender(DiagnosticsSection section)
        {
            Section = section;
        }

        public void Append(IDbSettings settings)
        {
            Section.AddGroup("Settings")
                .AddNode("AllowUpsertsOfSchemas", settings.AllowUpsertsOfSchemas)
                .AddNode("SynchronizeSchemaChanges", settings.SynchronizeSchemaChanges)
                .AddNode("MaxInsertManyBatchSize", settings.MaxInsertManyBatchSize)
                .AddNode("MaxUpdateManyBatchSize", settings.MaxUpdateManyBatchSize);
        }
    }
}