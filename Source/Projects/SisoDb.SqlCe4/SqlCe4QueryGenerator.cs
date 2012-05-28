using SisoDb.Dac;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : SqlServerQueryGenerator
    {
        public SqlCe4QueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder)
            : base(sqlStatements, sqlExpressionBuilder) { }
    }
}