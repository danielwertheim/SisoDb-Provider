using System;
using System.Transactions;

namespace SisoDb.SqlServer
{
    public static class Transactions
    {
        public static bool ActiveTransactionExists
        {
            get { return Transaction.Current != null; }
        }

        public static void SuppressOngoingTransactionWhile(Action suppressedAction)
        {
            using(new TransactionScope(TransactionScopeOption.Suppress))
            {
                suppressedAction.Invoke();
            }
        }
    }
}