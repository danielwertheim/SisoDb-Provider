namespace SisoDb
{
    public interface ISisoDbFactory
    {
        ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo);
    }
}