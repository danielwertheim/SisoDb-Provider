namespace SisoDb.Diagnostics.Appenders
{
    public class DbSettingsDiagnosticsAppender : IDiagnosticsAppender<IDbSettings>
    {
        protected readonly DiagnosticsInfo Info;

        public DbSettingsDiagnosticsAppender(DiagnosticsInfo info)
        {
            Info = info;
        }

        public void Append(IDbSettings settings)
        {
            Info.AddGroup("Settings")
                .AddNode("AllowUpsertsOfSchemas", settings.AllowUpsertsOfSchemas)
                .AddNode("SynchronizeSchemaChanges", settings.SynchronizeSchemaChanges)
                .AddNode("MaxInsertManyBatchSize", settings.MaxInsertManyBatchSize)
                .AddNode("MaxUpdateManyBatchSize", settings.MaxUpdateManyBatchSize);
        }
    }
}