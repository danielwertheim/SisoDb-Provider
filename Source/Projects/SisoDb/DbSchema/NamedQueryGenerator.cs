using System;
using System.Linq;
using System.Text;
using EnsureThat;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.DbSchema
{
    public class NamedQueryGenerator<T> : INamedQueryGenerator<T> where T : class
    {
        protected readonly IQueryBuilder<T> QueryBuilder;
        protected readonly IDbQueryGenerator DbQueryGenerator;
        protected readonly IDbDataTypeTranslator DataTypeTranslator;

        public NamedQueryGenerator(IQueryBuilder<T> queryBuilder, IDbQueryGenerator dbQueryGenerator, IDbDataTypeTranslator dataTypeTranslator)
        {
            QueryBuilder = queryBuilder;
            DbQueryGenerator = dbQueryGenerator;
            DataTypeTranslator = dataTypeTranslator;
        }

        public string Generate(string name, Action<IQueryBuilder<T>> spec)
        {
            Ensure.That(name, "name").IsNotNullOrWhiteSpace();
            Ensure.That(spec, "spec").IsNotNull();

            QueryBuilder.Clear();
            spec.Invoke(QueryBuilder);

            //Turns lamba expressions to a custom (serializable) node tree
            var query = QueryBuilder.Build();

            //Turns the parsed lambdas above to an internal SQL Expression tree and then to a SQL Query
            var dbQuery = DbQueryGenerator.GenerateQuery(query);

            const string format = "create procedure {0}\r\n{1}\r\nas\r\nbegin\r\n{2}\r\nend;";
            var parameters = GenerateInputParametersString(dbQuery);
            var body = GenerateBody(dbQuery);

            return string.Format(format, name, parameters, body);
        }

        protected virtual string GenerateInputParametersString(IDbQuery dbQuery)
        {
            const string paramFormat = "{0} {1}";

            return dbQuery.Parameters
                .Where(p => p.Name.StartsWith("@p"))
                .Select(p => string.Format(paramFormat, 
                    p.Name, 
                    DataTypeTranslator.ToDbType(p.Value.GetType())))
                .Aggregate((current, next) => string.Concat(current, ", ", Environment.NewLine, next));
        }

        protected virtual string GenerateBody(IDbQuery dbQuery)
        {
            var body = new StringBuilder();
            body.AppendLine("set nocount on;");
            body.AppendLine(GenerateBodyParametersString(dbQuery));
            body.Append(dbQuery.Sql);

            return body.ToString();
        }

        protected virtual string GenerateBodyParametersString(IDbQuery dbQuery)
        {
            const string paramFormat = "declare {0} as varchar({1}); set {0} = '{2}';";

            return dbQuery.Parameters
                .Where(p => p.Name.StartsWith("@mem") || p.Name.StartsWith("@inc"))
                .Select(p => string.Format(paramFormat,
                    p.Name,
                    p.Value.ToString().Length,
                    p.Value))
                .Aggregate((current, next) => string.Concat(current, Environment.NewLine, next));
        }
    }
}