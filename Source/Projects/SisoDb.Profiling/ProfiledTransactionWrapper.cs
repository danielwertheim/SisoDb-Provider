using System.Data;
using System.Data.Common;
using MvcMiniProfiler.Data;
using SisoDb.Dac.Profiling;

namespace SisoDb.Profiling
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