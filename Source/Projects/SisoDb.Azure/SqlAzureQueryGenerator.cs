using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.Azure
{
    public class SqlAzureQueryGenerator : DbQueryGenerator
    {
        public SqlAzureQueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder) 
            : base(sqlStatements, sqlExpressionBuilder) {}
    }
}