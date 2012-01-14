namespace SisoDb
{
	/// <summary>
	/// Factory used to create an <see cref="ISisoDatabase"/> which
	/// is the starting point to consume SisoDb.
	/// </summary>
    public interface ISisoDatabaseFactory
    {
        ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo);
    }
}