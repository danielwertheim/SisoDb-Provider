using System.Data.Common;

namespace SisoDb.MiniProfiler
{
    public class ProfiledTransactionWrapper : MvcMiniProfiler.Data.ProfiledDbTransaction, IWrappedTransaction
    {
        private DbTransaction transaction;
        public ProfiledTransactionWrapper(DbTransaction transaction, MvcMiniProfiler.Data.ProfiledDbConnection connection)
            : base(transaction, connection)
        {
            this.transaction = transaction;
        }

        public System.Data.IDbTransaction GetInnerTransaction()
        {
            return transaction;
        }
    }
}
