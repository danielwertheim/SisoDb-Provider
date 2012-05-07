namespace SisoDb.Diagnostics.Appenders
{
    public class DbSettingsAppender : IDiagnosticsContextAppender<IDbSettings>
    {
        protected readonly DiagnosticsContext Context;

        public DbSettingsAppender(DiagnosticsContext context)
        {
            Context = context;
        }

        public void Append(IDbSettings settings)
        {
            var section = Context.AddSection("Settings");
            section.AddNode("AllowUpsertsOfSchemas", settings.AllowUpsertsOfSchemas);
            section.AddNode("SynchronizeSchemaChanges", settings.SynchronizeSchemaChanges);
            section.AddNode("MaxInsertManyBatchSize", settings.MaxInsertManyBatchSize);
            section.AddNode("MaxUpdateManyBatchSize", settings.MaxUpdateManyBatchSize);
        }
    }
}