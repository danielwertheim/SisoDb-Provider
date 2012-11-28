using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : DbQueryGenerator
    {
        public SqlCe4QueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder)
            : base(sqlStatements, sqlExpressionBuilder) { }

        protected override IDbQuery CreateSqlQuery(IQuery query)
        {
            var sqlExpression = SqlExpressionBuilder.Process(query);
            var formatter = CreateSqlQueryFormatter(query, sqlExpression);
            var parameters = GenerateParameters(query, sqlExpression);

            if (query.HasNoDependencies())
                return new DbQuery(formatter.Format(SqlStatements.GetSql("QueryWithoutDependencies")), parameters, query.IsCacheable);

            return new DbQuery(formatter.Format(SqlStatements.GetSql("Query")), parameters, query.IsCacheable);
        }
    }
}