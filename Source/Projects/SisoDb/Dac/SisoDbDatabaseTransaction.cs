using System;
using System.Data;
using EnsureThat;

namespace SisoDb.Dac
{
    public abstract class SisoDbDatabaseTransaction : ISisoDbDatabaseTransaction
    {
        public IDbTransaction InnerTransaction { get; private set; }

        public bool Failed { get; private set; }

        protected SisoDbDatabaseTransaction(IDbTransaction transaction)
        {
            Ensure.That(transaction, "transaction").IsNotNull();
            InnerTransaction = transaction;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (InnerTransaction != null)
            {
                if (Failed)
                    InnerTransaction.Rollback();
                else
                    InnerTransaction.Commit();

                InnerTransaction.Dispose();
                InnerTransaction = null;
            }
        }

        public void MarkAsFailed()
        {
            Failed = true;
        }
    }
}