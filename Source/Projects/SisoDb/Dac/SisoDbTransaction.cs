using System;
using System.Transactions;
using EnsureThat;
using SisoDb.Resources;

namespace SisoDb.Dac
{
    public class SisoDbTransaction : ISisoDbTransaction
    {
        private readonly TransactionScopeOption _option;
        private TransactionScope _ts;

        public bool Failed { get; set; }

        public void MarkAsFailed()
        {
            if (_option == TransactionScopeOption.Suppress)
                throw new SisoDbException(ExceptionMessages.DbTransaction_MarkAsFailed_ForSuppressedTransaction);

            Failed = true;
        }

        private SisoDbTransaction(TransactionScopeOption option)
        {
            _option = option;
            _ts = new TransactionScope(_option, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
        }
        
        public void Dispose()
        {
            if (_ts != null)
            {
                if (!Failed)
                    _ts.Complete();

                _ts.Dispose();
                _ts = null;
            }
        }

        public static ISisoDbTransaction CreateRequired()
        {
            return new SisoDbTransaction(TransactionScopeOption.Required);
        }

        public static ISisoDbTransaction CreateSuppressed()
        {
            return new SisoDbTransaction(TransactionScopeOption.Suppress);
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

        public static void ExecuteRequired(Action<ISisoDbTransaction> action)
        {
            Ensure.That(action, "action").IsNotNull();

            using (var t = new SisoDbTransaction(TransactionScopeOption.Required))
            {
                try
                {
                    action.Invoke(t);
                }
                catch
                {
                    t.MarkAsFailed();
                    throw;
                }
            }
        }

        public static T ExecuteRequired<T>(Func<ISisoDbTransaction, T> action)
        {
            Ensure.That(action, "action").IsNotNull();
                        
            using (var t = new SisoDbTransaction(TransactionScopeOption.Required))
            {
                T r;

                try
                {
                    r = action.Invoke(t);
                }
                catch
                {
                    t.MarkAsFailed();
                    throw;
                }
                return r;
            }
        }

        public static void ExecuteSuppressed(Action action)
        {
            Ensure.That(action, "action").IsNotNull();

            using (new SisoDbTransaction(TransactionScopeOption.Suppress)) {
                action.Invoke();
            }
        }

        public static T ExecuteSuppressed<T>(Func<T> action)
        {
            Ensure.That(action, "action").IsNotNull();

            using (new SisoDbTransaction(TransactionScopeOption.Suppress)) {
                return action.Invoke();
            }
        }
    }
}