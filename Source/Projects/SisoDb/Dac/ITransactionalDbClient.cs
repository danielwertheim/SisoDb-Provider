namespace SisoDb.Dac
{
    public interface ITransactionalDbClient : IDbClient
    {
        ISisoTransaction Transaction { get; }
    }
}