using SisoDb.DbSchema;

namespace SisoDb.SqlAzure.DbSchema
{
    public class AzureDbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} null", name, dbType);
        }
    }
}