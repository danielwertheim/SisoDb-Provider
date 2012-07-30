namespace SisoDb.Serialization
{
    public class SerializerOptions
    {
        /// <summary>
        /// Determines if the deserialization should be performed in parallel.
        /// DEFAULT IS: true
        /// </summary>
        public bool DeserializeManyInParallel { get; set; }

        /// <summary>
        /// Determines the format of how DateTime should be represented.
        /// DEFAULT IS <see cref="DateSerializationModes.Iso8601"/>.
        /// </summary>
        public DateSerializationModes DateSerializationMode { get; set; }

        public SerializerOptions()
        {
            DeserializeManyInParallel = true;
            DateSerializationMode = DateSerializationModes.Iso8601;
        }
    }
}