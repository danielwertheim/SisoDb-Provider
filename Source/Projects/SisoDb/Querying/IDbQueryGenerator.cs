using SisoDb.Querying.Sql;

namespace SisoDb.Querying
{
    public interface IDbQueryGenerator
    {
        IDbQuery GenerateQuery(IQuery query);
        IDbQuery GenerateQueryReturningStrutureIds(IQuery query);
        IDbQuery GenerateQueryReturningCountOfStrutureIds(IQuery query);
    }
}