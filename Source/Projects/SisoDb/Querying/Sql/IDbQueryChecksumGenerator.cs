namespace SisoDb.Querying.Sql
{
    public interface IDbQueryChecksumGenerator 
    {
        string Generate(IDbQuery query);
    }
}