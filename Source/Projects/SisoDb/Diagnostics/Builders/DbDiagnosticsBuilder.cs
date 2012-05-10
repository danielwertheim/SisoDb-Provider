using EnsureThat;
using SisoDb.Diagnostics.Appenders;

namespace SisoDb.Diagnostics.Builders
{
    public class DbDiagnosticsBuilder : IDiagnosticsBuilder
    {
        protected readonly ISisoDatabase Database;

        public DbDiagnosticsBuilder(ISisoDatabase database)
        {
            Ensure.That(database, "database").IsNotNull();
            Database = database;
        }

        public virtual DiagnosticsInfo Build()
        {
            var diagnosticsInfo = new DiagnosticsInfo("Db: '{0}'", Database.Name);
            AppendConnectionInfoDiagnostics(diagnosticsInfo);
            AppendSerializerDiagnostics(diagnosticsInfo);
            AppendSettingsDiagnostics(diagnosticsInfo);
            AppendStructureSchemasDiagnostics(diagnosticsInfo);

            return diagnosticsInfo;
        }

        protected virtual void AppendConnectionInfoDiagnostics(DiagnosticsInfo info)
        {
            var appender = new ConnectionInfoDiagnosticsAppender(info);
            appender.Append(Database.ConnectionInfo);
        }

        protected virtual void AppendSerializerDiagnostics(DiagnosticsInfo info)
        {
            var appender = new SerializerDiagnosticsAppender(info);
            appender.Append(Database.Serializer);
        }

        protected virtual void AppendSettingsDiagnostics(DiagnosticsInfo info)
        {
            var appender = new DbSettingsDiagnosticsAppender(info);
            appender.Append(Database.Settings);
        }

        protected virtual void AppendStructureSchemasDiagnostics(DiagnosticsInfo info)
        {
            var appender = new StructureSchemasDiagnosticsAppender(info);
            appender.Append(Database.StructureSchemas);
        }
    }
}