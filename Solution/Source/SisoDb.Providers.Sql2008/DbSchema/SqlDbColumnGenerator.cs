using SisoDb.DbSchema;

namespace SisoDb.Sql2008.DbSchema
{
    public class SqlDbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} sparse null", name, dbType);
        }
    }
}