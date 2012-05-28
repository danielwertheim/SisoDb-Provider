using SisoDb.Dac;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;

namespace SisoDb.Sql2012
{
    public class Sql2012QueryGenerator : SqlServerQueryGenerator
    {
        public Sql2012QueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder) 
            : base(sqlStatements, sqlExpressionBuilder) {}
    }
}