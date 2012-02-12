using System;
using System.Transactions;
using SisoDb.Resources;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4DbTransaction : ISisoTransaction
    {
        private readonly TransactionScopeOption _option;
        private TransactionScope _ts;

        public bool Failed { get; private set; }

        public void MarkAsFailed()
        {
            if (_option == TransactionScopeOption.Suppress)
                throw new SisoDbException(ExceptionMessages.DbTransaction_MarkAsFailed_ForSuppressedTransaction);

            Failed = true;
        }

        private SqlCe4DbTransaction(TransactionScopeOption option)
        {
            _option = option;
            _ts = new TransactionScope(_option, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_ts != null)
            {
                if (!Failed)
                    _ts.Complete();

                _ts.Dispose();
                _ts = null;
            }
        }

        public static ISisoTransaction CreateRequired()
        {
            return new SqlCe4DbTransaction(TransactionScopeOption.Required);
        }

        public static ISisoTransaction CreateSuppressed()
        {
            return new SqlCe4DbTransaction(TransactionScopeOption.Suppress);
        }

        public void Try(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
                MarkAsFailed();
                throw;
            }
        }

        public T Try<T>(Func<T> action)
        {
            try
            {
                return action.Invoke();
            }
            catch
            {
                MarkAsFailed();
                throw;
            }
        }
    }
}