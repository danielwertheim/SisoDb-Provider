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
                .AddNode("AllowDynamicSchemaCreation", settings.AllowDynamicSchemaCreation)
                .AddNode("AllowDynamicSchemaUpdates", settings.AllowDynamicSchemaUpdates)
                .AddNode("MaxInsertManyBatchSize", settings.MaxInsertManyBatchSize)
                .AddNode("MaxUpdateManyBatchSize", settings.MaxUpdateManyBatchSize);
        }
    }
}