using SisoDb.Querying.Sql;

namespace SisoDb.Querying
{
    public interface IDbQueryGenerator
    {
        DbQuery GenerateQuery(IQuery query);

        DbQuery GenerateQueryReturningStrutureIds(IQuery query);
    }
}