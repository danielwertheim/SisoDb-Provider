namespace SisoDb.Dac
{
    public interface ITransactionalDbClient : IDbClient
    {
        ISisoDbDatabaseTransaction Transaction { get; }
    }
}