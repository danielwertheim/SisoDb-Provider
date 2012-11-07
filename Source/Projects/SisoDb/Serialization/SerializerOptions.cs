namespace SisoDb.Serialization
{
    public class SerializerOptions
    {
        /// <summary>
        /// Determines the format of how DateTime should be represented.
        /// DEFAULT IS <see cref="DateSerializationModes.Iso8601"/>.
        /// </summary>
        public DateSerializationModes DateSerializationMode { get; set; }

        public SerializerOptions()
        {
            DateSerializationMode = DateSerializationModes.Iso8601;
        }
    }
}