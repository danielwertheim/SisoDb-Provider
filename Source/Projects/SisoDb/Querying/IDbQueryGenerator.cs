using SisoDb.Querying.Sql;

namespace SisoDb.Querying
{
    public interface IDbQueryGenerator
    {
        SqlQuery GenerateQuery(IQuery query);

        SqlQuery GenerateQueryReturningStrutureIds(IQuery query);
    }
}