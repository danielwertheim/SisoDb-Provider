namespace SisoDb.Spatial
{
    public static class SessionExtensions
    {
        public static ISisoSpatial Spatial(this ISession session)
        {
            return new SqlServerSisoSpatial(session.ExecutionContext);
        }
    }
}