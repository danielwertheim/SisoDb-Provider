using System.Data;
using SisoDb.Dac;

namespace SisoDb.Sql2012.Dac
{
    public class Sql2012DbTransaction : SisoDbDatabaseTransaction
    {
        public Sql2012DbTransaction(IDbTransaction transaction) : base(transaction) {}
    }
}