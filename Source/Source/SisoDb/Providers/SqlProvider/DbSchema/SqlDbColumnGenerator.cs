namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal class SqlDbColumnGenerator : ISqlDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} sparse null", name, dbType);
        }
    }
}