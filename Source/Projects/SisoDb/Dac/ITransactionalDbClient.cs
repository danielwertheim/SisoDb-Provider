using System;
using System.Data;

namespace SisoDb.Dac
{
    public interface ITransactionalDbClient : IDbClient
    {
        IDbTransaction Transaction { get; }
        bool IsFailed { get; }
        Action AfterCommit { set; }
        Action AfterRollback { set; }
        void MarkAsFailed();
    }
}