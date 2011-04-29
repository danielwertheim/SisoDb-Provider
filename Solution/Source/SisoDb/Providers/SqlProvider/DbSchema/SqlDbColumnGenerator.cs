using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public class SqlDbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} sparse null", name, dbType);
        }
    }
}