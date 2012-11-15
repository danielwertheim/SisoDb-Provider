namespace SisoDb.Spatial
{
    public static class SessionExtensions
    {
        public static ISisoGeographical Spatials(this ISession session)
        {
            return new SqlServerSisoGeographical(session.ExecutionContext);
        }
    }
}