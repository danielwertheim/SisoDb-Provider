using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : DbQueryGenerator
    {
        public SqlCe4QueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder)
            : base(sqlStatements, sqlExpressionBuilder) { }
    }
}