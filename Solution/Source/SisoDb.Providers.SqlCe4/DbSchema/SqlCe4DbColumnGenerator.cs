using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.SqlCe4.DbSchema
{
    public class SqlCe4DbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} sparse null", name, dbType);
        }
    }
}