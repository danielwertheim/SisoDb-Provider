using SisoDb.Querying.Sql;

namespace SisoDb.Querying
{
    public interface IDbQueryGenerator
    {
        SqlQuery GenerateQuery(IQueryCommand queryCommand);

        SqlQuery GenerateQueryReturningStrutureIds(IQueryCommand queryCommand);
    }
}