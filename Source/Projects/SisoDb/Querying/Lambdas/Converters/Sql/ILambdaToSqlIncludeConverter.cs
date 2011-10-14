using System.Collections.Generic;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public interface ILambdaToSqlIncludeConverter : IParsedLambdaConverter<IList<SqlInclude>>
    {}
}