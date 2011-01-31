namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal interface ISqlDbColumnGenerator
    {
        string ToSql(string name, string dbType);
    }
}