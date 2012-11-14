namespace SisoDb.Spatial
{
    public static class SessionExtensions
    {
        public static ISisoSpatial Spatials(this ISession session)
        {
            return new SqlServerSisoSpatial(session.ExecutionContext);
        }
    }
}