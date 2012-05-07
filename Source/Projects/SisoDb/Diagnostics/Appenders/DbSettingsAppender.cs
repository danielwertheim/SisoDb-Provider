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
            var group = section.AddGroup("Settings");
            group.AddNode("AllowUpsertsOfSchemas", settings.AllowUpsertsOfSchemas);
            group.AddNode("SynchronizeSchemaChanges", settings.SynchronizeSchemaChanges);
            group.AddNode("MaxInsertManyBatchSize", settings.MaxInsertManyBatchSize);
            group.AddNode("MaxUpdateManyBatchSize", settings.MaxUpdateManyBatchSize);
        }
    }
}