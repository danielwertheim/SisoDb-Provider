namespace SisoDb.Spatial
{
    public static class SessionExtensions
    {
        public static ISisoSpatials Spatials(this ISession session)
        {
            return new SqlServerSisoSpatials(session.ExecutionContext);
        }
    }
}