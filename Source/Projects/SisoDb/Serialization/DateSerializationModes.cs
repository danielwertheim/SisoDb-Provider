using System;

namespace SisoDb.Serialization
{
    [Serializable]
    public enum DateSerializationModes
    {
        Iso8601,
        TimestampOffset
    }
}