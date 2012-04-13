using System.Data;

namespace SisoDb.Dac
{
    public interface ITransactionalDbClient : IDbClient
    {
        IDbTransaction Transaction { get; }
        bool Failed { get; }
        void MarkAsFailed();
    }
}