namespace SisoDb.Providers.DbSchema
{
    public interface IDbColumnGenerator
    {
        string ToSql(string name, string dbType);
    }
}