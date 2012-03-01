namespace SisoDb.Core
{
    public static class CastExtensions //TODO: Move to NCore
    {
        public static T CastAs<T>(this object obj) where T : class
        {
            return obj as T;
        }
    }
}