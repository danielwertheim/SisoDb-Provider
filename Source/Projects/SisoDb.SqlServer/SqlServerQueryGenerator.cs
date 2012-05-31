using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlServer
{
    public abstract class SqlServerQueryGenerator : DbQueryGenerator
    {
        protected SqlServerQueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder) 
            : base(sqlStatements, sqlExpressionBuilder) {}
    }
}