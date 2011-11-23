namespace SisoDb.Core
{
    public static class StringExtensions
    {
        //TODO: Move to NCore
        public static string ToStringOrNull(this object obj)
        {
            if (obj == null)
                return null;

            return obj.ToString();
        }
    }
}