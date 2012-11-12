namespace SisoDb.Diagnostics.Appenders
{
    public class ConnectionInfoDiagnosticsAppender : IDiagnosticsAppender<ISisoConnectionInfo>
    {
        protected readonly DiagnosticsInfo Info;

        public ConnectionInfoDiagnosticsAppender(DiagnosticsInfo info)
        {
            Info = info;
        }

        public void Append(ISisoConnectionInfo connectionInfo)
        {
            Info.AddGroup("ConnectionInfo")
                .AddNode("DbName", connectionInfo.DbName)
                .AddNode("ProviderType", connectionInfo.ProviderType)
                .AddNode("ClientConnectionString", connectionInfo.ClientConnectionString)
                .AddNode("ServerConnectionString", connectionInfo.ServerConnectionString);
        }
    }
}