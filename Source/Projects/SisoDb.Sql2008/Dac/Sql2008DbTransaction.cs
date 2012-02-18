using System.Data;
using SisoDb.Dac;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008DbTransaction : SisoDbDatabaseTransaction
    {
        public Sql2008DbTransaction(IDbTransaction transaction) : base(transaction) { }
    }
}