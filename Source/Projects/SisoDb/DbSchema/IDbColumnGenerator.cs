namespace SisoDb.DbSchema
{
    public interface IDbColumnGenerator
    {
        string ToSql(string name, string dbType);
    }
}