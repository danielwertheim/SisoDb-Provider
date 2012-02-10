using System;
using System.Transactions;
using SisoDb.Resources;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008DbTransaction : ISisoTransaction
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

        private Sql2008DbTransaction(TransactionScopeOption option)
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
            return new Sql2008DbTransaction(TransactionScopeOption.Required);
        }

        public static ISisoTransaction CreateSuppressed()
        {
            return new Sql2008DbTransaction(TransactionScopeOption.Suppress);
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