namespace SisoDb.Querying
{
    public static class SortingExtensions
    {
        public static T Asc<T>(this T t)
        {
            return t;
        }

        public static T Desc<T>(this T t)
        {
            return t;
        }
    }
}