namespace SisoDb.Dac
{
    public interface ITransactionalDbClient : IDbClient
    {
        bool Failed { get; }
        void MarkAsFailed();
    }
}