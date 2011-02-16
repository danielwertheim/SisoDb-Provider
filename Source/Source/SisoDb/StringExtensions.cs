namespace SisoDb
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string Inject(this string format, params object[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }

        public static string Inject(this string format, params string[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }
    }
}