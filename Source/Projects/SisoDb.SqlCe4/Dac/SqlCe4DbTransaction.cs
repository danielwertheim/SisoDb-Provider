using System.Data;
using SisoDb.Dac;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4DbTransaction : SisoDbDatabaseTransaction
    {
        public SqlCe4DbTransaction(IDbTransaction transaction) : base(transaction) { }
    }
}