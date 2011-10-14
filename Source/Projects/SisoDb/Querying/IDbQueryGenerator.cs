namespace SisoDb.Querying.Sql
{
    public interface IDbQueryGenerator
    {
        SqlQuery GenerateQuery(IQueryCommand queryCommand);

        SqlQuery GenerateWhereQuery(IQueryCommand queryCommand);
    }
}