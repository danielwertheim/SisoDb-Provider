using System;
using System.Transactions;
using SisoDb.Resources;

namespace SisoDb.Dac
{
    public class SisoDbTransaction : ISisoDbTransaction
    {
        private readonly TransactionScopeOption _option;
        private TransactionScope _ts;
        //private DependentTransaction _dt;
        private Transaction _old;

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
            _old = Transaction.Current;

            //if (option == TransactionScopeOption.Suppress)
            //{
            //    _ts = new TransactionScope(_option, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, EnterpriseServicesInteropOption.None);
            //    return;
            //}

            _ts = new TransactionScope(_option, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, EnterpriseServicesInteropOption.None);

            //if (_old != null)
            //{
            //    _dt = _old.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
            //    Transaction.Current = _dt;
            //}
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            //if (_dt != null)
            //{
            //    if (Failed)
            //        _dt.Rollback();

            //    _dt.Complete();
            //    _dt.Dispose();
            //    _dt = null;
            //}

            if (_ts != null)
            {
                if (!Failed && _option != TransactionScopeOption.Suppress)
                    _ts.Complete();

                _ts.Dispose();
                _ts = null;
            }

            Transaction.Current = _old;
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

        //public static void ExecuteRequired(Action<ISisoDbTransaction> action)
        //{
        //    Ensure.That(action, "action").IsNotNull();

        //    using (var t = new SisoDbTransaction(TransactionScopeOption.Required))
        //    {
        //        try
        //        {
        //            action.Invoke(t);
        //        }
        //        catch
        //        {
        //            t.MarkAsFailed();
        //            throw;
        //        }
        //    }
        //}

        //public static T ExecuteRequired<T>(Func<ISisoDbTransaction, T> action)
        //{
        //    Ensure.That(action, "action").IsNotNull();

        //    using (var t = new SisoDbTransaction(TransactionScopeOption.Required))
        //    {
        //        T r;

        //        try
        //        {
        //            r = action.Invoke(t);
        //        }
        //        catch
        //        {
        //            t.MarkAsFailed();
        //            throw;
        //        }
        //        return r;
        //    }
        //}

        //public static void ExecuteSuppressed(Action action)
        //{
        //    Ensure.That(action, "action").IsNotNull();

        //    using (new SisoDbTransaction(TransactionScopeOption.Suppress)) {
        //        action.Invoke();
        //    }
        //}

        //public static T ExecuteSuppressed<T>(Func<T> action)
        //{
        //    Ensure.That(action, "action").IsNotNull();

        //    using (new SisoDbTransaction(TransactionScopeOption.Suppress))
        //    {
        //        return action.Invoke();
        //    }
        //}
    }
}