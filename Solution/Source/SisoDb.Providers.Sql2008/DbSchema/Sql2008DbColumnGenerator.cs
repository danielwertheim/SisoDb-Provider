using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    public class Sql2008DbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} sparse null", name, dbType);
        }
    }
}