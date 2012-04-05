using System.Data;
using System.Data.Common;
using SisoDb.Dac.Profiling;
using StackExchange.Profiling.Data;

namespace SisoDb.MiniProfiler
{
    public class ProfiledTransactionWrapper : ProfiledDbTransaction, IWrappedTransaction
    {
        protected readonly DbTransaction Transaction;

        public ProfiledTransactionWrapper(DbTransaction transaction, ProfiledDbConnection connection)
            : base(transaction, connection)
        {
            Transaction = transaction;
        }

        public virtual IDbTransaction GetInnerTransaction()
        {
            return Transaction;
        }
    }
}