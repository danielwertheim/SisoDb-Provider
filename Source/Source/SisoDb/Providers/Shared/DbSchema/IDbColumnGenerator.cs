namespace SisoDb.Providers.Shared.DbSchema
{
    public interface IDbColumnGenerator
    {
        string ToSql(string name, string dbType);
    }
}