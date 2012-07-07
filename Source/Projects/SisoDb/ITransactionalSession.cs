namespace SisoDb
{
    public interface ITransactionalSession : ISession
    {
        bool Failed { get; }
        void MarkAsFailed();
    }
}