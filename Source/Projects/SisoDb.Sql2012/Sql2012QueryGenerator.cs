using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.Sql2012
{
    public class Sql2012QueryGenerator : DbQueryGenerator
    {
        public Sql2012QueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder) 
            : base(sqlStatements, sqlExpressionBuilder) {}
    }
}