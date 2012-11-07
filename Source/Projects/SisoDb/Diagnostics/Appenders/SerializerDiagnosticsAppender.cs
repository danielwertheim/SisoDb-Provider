using SisoDb.Serialization;

namespace SisoDb.Diagnostics.Appenders
{
    public class SerializerDiagnosticsAppender : IDiagnosticsAppender<ISisoDbSerializer>
    {
        protected readonly DiagnosticsInfo Info;

        public SerializerDiagnosticsAppender(DiagnosticsInfo info)
        {
            Info = info;
        }

        public void Append(ISisoDbSerializer serializer)
        {
            Info.AddGroup("Serializer")
                .AddNode("Type", serializer.GetType().Name)
                .AddNode("DateSerializationMode", serializer.Options.DateSerializationMode.ToString());
        }
    }
}