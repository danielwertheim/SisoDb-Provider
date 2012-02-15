using System;
using System.Transactions;
using SisoDb.Resources;

namespace SisoDb.Sql2012.Dac
{
    public class Sql2012DbTransaction : ISisoTransaction
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

        private Sql2012DbTransaction(TransactionScopeOption option)
        {
            _option = option;
            _ts = new TransactionScope(_option, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, EnterpriseServicesInteropOption.None);
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
            return new Sql2012DbTransaction(TransactionScopeOption.Required);
        }

        public static ISisoTransaction CreateSuppressed()
        {
            return new Sql2012DbTransaction(TransactionScopeOption.Suppress);
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