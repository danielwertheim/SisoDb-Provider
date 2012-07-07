using SisoDb.Dac;

namespace SisoDb
{
    public interface ITransactionalSession : ISession
    {
        ITransactionalDbClient TransactionalDbClient { get; }
    }
}