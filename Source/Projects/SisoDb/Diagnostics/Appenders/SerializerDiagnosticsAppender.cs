using SisoDb.Serialization;

namespace SisoDb.Diagnostics.Appenders
{
    public class SerializerDiagnosticsAppender : IDiagnosticsAppender<ISisoSerializer>
    {
        protected readonly DiagnosticsInfo Info;

        public SerializerDiagnosticsAppender(DiagnosticsInfo info)
        {
            Info = info;
        }

        public void Append(ISisoSerializer serializer)
        {
            Info.AddGroup("Serializer")
                .AddNode("Type", serializer.GetType().Name);
        }
    }
}