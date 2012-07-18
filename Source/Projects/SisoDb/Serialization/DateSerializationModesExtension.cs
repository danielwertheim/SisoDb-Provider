using SisoDb.NCore;

namespace SisoDb.Serialization
{
    internal static class DateSerializationModesExtension
    {
        internal static JsonDateHandler ToServiceStackValue(this DateSerializationModes value)
        {
            switch (value)
            {
                case DateSerializationModes.Iso8601:
                    return JsonDateHandler.ISO8601;
                case DateSerializationModes.TimestampOffset:
                    return JsonDateHandler.TimestampOffset;
            }

            throw new SisoDbNotSupportedException("The datetime serialization mode: '{0}' is not supported.".Inject(value.ToString()));
        }
    }
}