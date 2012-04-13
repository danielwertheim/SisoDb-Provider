using SisoDb.Dac;
using SisoDb.Querying;

namespace SisoDb.Sql2012
{
    public class Sql2012QueryGenerator : DbQueryGenerator
    {
        public Sql2012QueryGenerator(ISqlStatements sqlStatements) : base(sqlStatements) {}
    }
}