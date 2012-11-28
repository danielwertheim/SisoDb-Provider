namespace SisoDb.Azure
{
    public class SqlAzureSession : DbSession
    {
        internal SqlAzureSession(ISisoDatabase db) : base(db)
        {}
    }
}